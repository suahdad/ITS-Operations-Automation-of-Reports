﻿Imports System.Data
Imports ADODB
Imports Reports
Public Class Vessel
    Implements IReports.Vessel

    Sub New(Registry As String, Connection As Connection)
        vslUnits = New Units(Registry, Connection)

        Retrieve(Registry, Connection)
        strRegistry = Registry
        vslConnection = Connection
    End Sub
    Private strRegistry As String
    Private vslConnection As Connection
    Private dtVessel As DataTable
    Private dtContainers As DataTable
    Private vslUnits As Units
    Enum Vessel
        Name
        LineOP
        IBVoyage
        OBVoyage
        Registry
        Berth
        ETA
        ATA
        ATD
        StartWork
        EndWork
        LastContrDisch
    End Enum
    Public ReadOnly Property Name As String Implements IReports.Vessel.Name
        Get
            Name = dtVessel.Rows(0)(Vessel.Name).ToString
        End Get
    End Property

    Public ReadOnly Property Registry As String Implements IReports.Vessel.Registry
        Get
            Registry = dtVessel.Rows(0)(Vessel.Registry).ToString
        End Get
    End Property

    Public ReadOnly Property InboundVoyage As String Implements IReports.Vessel.InboundVoyage
        Get
            InboundVoyage = dtVessel.Rows(0)(Vessel.IBVoyage).ToString
        End Get
    End Property

    Public ReadOnly Property OutboundVoyage As String Implements IReports.Vessel.OutboundVoyage
        Get
            OutboundVoyage = dtVessel.Rows(0)(Vessel.OBVoyage).ToString
        End Get
    End Property

    Public ReadOnly Property BerthWindow As String Implements IReports.Vessel.BerthWindow
        Get
            BerthWindow = dtVessel.Rows(0)(Vessel.Berth).ToString
        End Get
    End Property

    Public ReadOnly Property ATA As Date Implements IReports.Vessel.ATA
        Get
            ATA = dtVessel.Rows(0)(Vessel.ATA).ToString
        End Get
    End Property

    Public ReadOnly Property ATD As Date Implements IReports.Vessel.ATD
        Get
            ATD = dtVessel.Rows(0)(Vessel.ATD).ToString
        End Get
    End Property

    Public ReadOnly Property ETA As Date Implements IReports.Vessel.ETA
        Get
            ETA = dtVessel.Rows(0)(Vessel.ETA).ToString
        End Get
    End Property

    Public ReadOnly Property ETD As Date Implements IReports.Vessel.ETD
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    Public ReadOnly Property StartWork As Date Implements IReports.Vessel.StartWork
        Get
            StartWork = dtVessel.Rows(0)(Vessel.StartWork).ToString()
        End Get
    End Property

    Public ReadOnly Property EndWork As Date Implements IReports.Vessel.EndWork
        Get
            EndWork = dtVessel.Rows(0)(Vessel.EndWork).ToString()
        End Get
    End Property

    Public ReadOnly Property FirstContainerDischarged As Date Implements IReports.Vessel.FirstContainerDischarged
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    Public ReadOnly Property LastContainerDischarged As Date Implements IReports.Vessel.LastContainerDischarged
        Get
            LastContainerDischarged = dtVessel.Rows(0)(Vessel.LastContrDisch).ToString()
        End Get
    End Property

    Public ReadOnly Property FirstContainerLoaded As Date Implements IReports.Vessel.FirstContainerLoaded
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    Public ReadOnly Property LastContainerLoaded As Date Implements IReports.Vessel.LastContainerLoaded
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    Public ReadOnly Property LineOperators() As String Implements IReports.Vessel.LineOperator
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    Public ReadOnly Property Owner As String Implements IReports.Vessel.Owner
        Get
            Owner = dtVessel.Rows(0)(Vessel.LineOP)
        End Get
    End Property

    Public ReadOnly Property Units As Units Implements IReports.Vessel.Units
        Get
            Units = vslUnits
        End Get
    End Property

    Public ReadOnly Property Connection As Connection Implements IReports.Vessel.Connection
        Get
            Connection = vslConnection
        End Get
    End Property

    Public Sub Retrieve(Registry As String, Connection As ADODB.Connection) Implements IReports.Vessel.Retrieve
        Dim rsContainers As New ADODB.Recordset
        Dim DataAdapter As New OleDb.OleDbDataAdapter
        Dim strSQLVessel As String

        dtVessel = New DataTable

        strSQLVessel =
        "select 'MV '+ vsl.name as 'Vessel Name'
        ,biz.[id] as 'Line Operator'
        ,ib_vyg 'I/B Voyage Number' 
        ,ob_vyg 'O/B Voyage Number'
        ,flex_string01 as 'Registry Number'
        ,flex_string02 as 'Pier Berth (NCT)'
        ,ata as 'Actual Time of Arrival (ATA)'
        ,atd as 'Actual Time of Departure (ATD)'
		,eta as 'Estimated Time of Arrival'
        ,[start_work] as 'Time Operation Commenced'
        ,[end_work] as 'Time of Completion'
        ,[time_discharge_complete] as 'Time of Last Contr. Discharged'

        FROM [apex].[dbo].[vsl_vessel_visit_details] vvd
        inner join 
        [vsl_vessels] vsl
        on vsl.gkey = vvd.vessel_gkey 
        inner join
        [argo_carrier_visit] acv
        on cvcvd_gkey = vvd_gkey
        inner join
        [argo_visit_details] avd
        on cvcvd_gkey = avd.gkey
        inner join
        [ref_bizunit_scoped] biz
        on [operator_gkey] = biz.gkey

        where acv.id = '" & Registry & "'"
        rsContainers.Open(strSQLVessel, Connection)
        DataAdapter.Fill(dtVessel, rsContainers)
        rsContainers.Close()
    End Sub

    Public Function TEU(Optional Condition As String = "") As Double Implements IReports.Vessel.TEU
        Dim dvContainers As New DataView()
    End Function

    Public Function Boxes(Optional Condition As String = "") As Long Implements IReports.Vessel.Boxes
        Throw New NotImplementedException()
    End Function
End Class