﻿Imports System.Data
Imports ADODB
Imports Reports

Public Class Units
    Implements IReports.Units

    Sub New(Registry As String, ByRef Connection As Connection)

        ' This call is required by the designer.
        Retrieve(Registry, Connection)
        strRegistry = Registry
        untConnection = Connection
        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Private strRegistry As String
    Private dsContainers As DataSet
    Private untConnection As Connection
    Public Enum UnitDetails
        ContNum
        NomLength
        IsoGrp
        FreightKnd
        LineOP
        ReqPowers
        Hazard
        IMDG
        IsoCode
        ctrTyp
    End Enum
    Public ReadOnly Property Registry As String Implements IReports.Units.Registry
        Get
            Registry = strRegistry
        End Get
    End Property

    Public ReadOnly Property Connection As Connection Implements IReports.Units.Connection
        Get
            Connection = untConnection
        End Get
    End Property

    Public ReadOnly Property Containers As DataSet Implements IReports.Units.Containers
        Get
            Containers = dsContainers
        End Get
    End Property

    Private Function getMilTime(strLDate As String) As String
        Dim dteDate As DateTime

        dteDate = Convert.ToDateTime(strLDate)
        getMilTime = dteDate.ToString("HHmm\H MM/dd/yyyy")
    End Function

    Private Sub tagCtrTyp(ByRef dtContr As DataTable)
        Dim colCtrTyp As New DataColumn
        colCtrTyp.ColumnName = "ctrTyp"

        dtContr.Columns.Add(colCtrTyp)
        For Each row As DataRow In dtContr.Rows
            If row("requires_power") = 1 Then
                row("ctrTyp") = "Rfr"
            Else
                Select Case row("iso_group").ToString.Substring(0, 1)
                    Case "P"
                        row("ctrTyp") = "FRk"
                    Case "U"
                        row("ctrTyp") = "Opn"
                    Case "T"
                        row("ctrTyp") = "Tnk"
                    Case Else
                        row("ctrTyp") = "Dry"
                End Select
            End If
        Next
    End Sub

    Public Sub Retrieve(Vessel As String, Connection As ADODB.Connection) Implements IReports.Units.Retrieve
        Dim strSQl(1) As String '0 = Inbound | 1 = Outbound
        Dim datAdapt As New OleDb.OleDbDataAdapter
        Dim rsUnits As New ADODB.Recordset
        Dim ibTable As New DataTable
        Dim obTable As New DataTable

        dsContainers = New DataSet
        dsContainers.Tables.Add(ibTable)
        dsContainers.Tables.Add(obTable)

        'Get I/B Containers
        strSQl(0) = "Select unit.[id]
,[nominal_length]
,reqt.[iso_group]
,[freight_kind]
,lin.[id] as 'line_op'
,[requires_power]
,[hazardous]
,[imdg_types]
,reqt.[id] as 'iso_code'

from 
[inv_unit] unit
inner join
[inv_unit_equip] ueqp
on unit.[gkey] = ueqp.[unit_gkey]
inner join
[inv_goods] gds
on goods = gds.[gkey]
inner join
[ref_bizunit_scoped] lin
on line_op = lin.gkey
inner join
[ref_equipment] reqp
on ueqp.eq_gkey = reqp.gkey
inner join 
[ref_equip_type] reqt
on reqp.eqtyp_gkey = reqt.gkey
inner join 
[inv_unit_fcy_visit] ufv 
on unit.gkey = ufv.unit_gkey
inner join
[argo_carrier_visit] acv
on [actual_ib_cv] = acv.gkey


where acv.id = '" & Vessel & "' and pod1_gkey = 1"

        'Get O/B Containers
        strSQl(1) = "Select unit.[id]
,[nominal_length]
,reqt.[iso_group]
,[freight_kind]
,lin.[id] as 'line_op'
,[requires_power]
,[hazardous]
,[imdg_types]
,reqt.[id] as 'iso_code'

from 
[inv_unit] unit
inner join
[inv_unit_equip] ueqp
on unit.[gkey] = ueqp.[unit_gkey]
inner join
[inv_goods] gds
on goods = gds.[gkey]
inner join
[ref_bizunit_scoped] lin
on line_op = lin.gkey
inner join
[ref_equipment] reqp
on ueqp.eq_gkey = reqp.gkey
inner join 
[ref_equip_type] reqt
on reqp.eqtyp_gkey = reqt.gkey
inner join 
[inv_unit_fcy_visit] ufv 
on unit.gkey = ufv.unit_gkey
inner join
[argo_carrier_visit] acv
on [actual_ob_cv] = acv.gkey


where acv.id = '" & Vessel & "' and category <> 'THRGH'"

        For count As Integer = 0 To 1
            rsUnits.Open(strSQl(count), Connection)
            datAdapt.Fill(dsContainers.Tables(count), rsUnits)
            tagCtrTyp(dsContainers.Tables(count))
            rsUnits.Close()
        Next


    End Sub

    Public Function Count(Containers As DataTable, Condition As String) As Double Implements IReports.Units.Count
        Dim dvContainers As New DataView
        dvContainers.Table = Containers
        dvContainers.RowFilter = Condition

        Count = dvContainers.Count
    End Function
End Class
