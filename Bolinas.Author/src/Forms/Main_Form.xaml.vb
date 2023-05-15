#Region "IMPORTS"

Imports SMT.DotNet.AppConsole
Imports SMT.DotNet.Serialization
Imports SMT.DotNet.Serialization.XML
Imports SMT.DotNet.Utility
Imports SMT.DotNet.Utility.mConversionsAndSuch
Imports SMT.Multimedia.Formats.AC3
Imports SMT.Multimedia.DirectShow
Imports SMT.Multimedia.Filters.nVidia
Imports SMT.Multimedia.Filters.SMT.Keystone
Imports SMT.Multimedia.Filters.SMT.AMTC
Imports SMT.Multimedia.Filters.SMT.L21G
Imports SMT.Multimedia.GraphConstruction
Imports SMT.Multimedia.Formats.DVD.IFO
Imports SMT.Multimedia.Enums
Imports SMT.Multimedia.Players
Imports SMT.Multimedia.Players.DVD
Imports SMT.Multimedia.Players.DVD.Classes
Imports SMT.Multimedia.Players.DVD.Enums
Imports SMT.Multimedia.Players.DVD.Structures
Imports SMT.Multimedia.UI.WPF.Viewer
Imports SMT.Multimedia.Utility.Timecode
Imports SMT.Win
Imports SMT.Win.EventLog
Imports SMT.Win.ProcessExecution
Imports SMT.Win.WinAPI.Constants
Imports System.Drawing
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Threading
Imports System.Windows.Interop
Imports System.ComponentModel

#End Region 'IMPORTS

Class Main_Form

#Region "PROPERTIES"

    Public ReadOnly Property SplashBitmap() As Bitmap
        Get
            Dim str As Stream = Me.GetType.Module.Assembly.GetManifestResourceStream("RCDb.Bolinas.Author.Bolinas_Author_Splash.png")
            Return New Bitmap(str)
        End Get
    End Property
    Private ReadOnly Property Handle() As IntPtr
        Get
            If _Handle = Nothing Then
                _Handle = New WindowInteropHelper(Me).Handle
            End If
            Return _Handle
        End Get
    End Property
    Private _Handle As IntPtr
    Private ProjectPath As String
    Private WithEvents AutoSaveTimer As System.Windows.Forms.Timer

#End Region 'PROPERTIES

#Region "CONSTRUCTOR"

    Public Sub New()
        MyBase.new()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        Try
            If String.IsNullOrEmpty(My.Settings.RUN_UPGRADE) OrElse My.Settings.RUN_UPGRADE Then
                My.Settings.Upgrade()
                My.Settings.RUN_UPGRADE = False
                My.Settings.Save()
            End If

            'Check to see if app is running
            Dim assemblyName As String = Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            If RunningInstanceCheck.IsProcessRunning(assemblyName) Then
                MessageBox.Show("Only one instance of this application may be run at a time.", My.Settings.APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Process.GetCurrentProcess.Kill()
            End If

            ''check version of keystone and bmdl
            'Dim FVI As FileVersionInfo
            'If Not File.Exists("C:\Program Files\Common Files\SMT Shared\Filters\keystone_SD.ax") Then Throw New Exception("Keystone is not installed correctly.")
            'FVI = FileVersionInfo.GetVersionInfo("C:\Program Files\Common Files\SMT Shared\Filters\keystone_SD.ax")
            'If FVI.FileVersion <> "1.2.0.0" Then
            '    MessageBox.Show("The wrong version of SMT Keystone is installed. Contact SMT for support.", My.Settings.APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation)
            '    Process.GetCurrentProcess.Kill()
            'End If

            'Show Splash Screen
            Dim ss As New Bolinas_SplashScreen(True, SplashBitmap)
            Dim dr As Windows.Forms.DialogResult = ss.ShowDialog
            Select Case dr
                Case Windows.Forms.DialogResult.Yes

                Case Windows.Forms.DialogResult.Abort
                    Debug.WriteLine("Problem with splash screen.")
            End Select

            Dim args() As String = Environment.GetCommandLineArgs()
            For Each a As String In args
                If InStr(a.ToLower, My.Settings.PROJECT_EXT) Then
                    ProjectPath = a
                ElseIf InStr(a.ToLower, "video_ts.ifo") Then
                    'm_DVDPath = a
                End If
            Next

        Catch ex As Exception
            MessageBox.Show("Problem with Main(). Error: " & ex.Message, My.Settings.APPLICATION_NAME, MessageBoxButton.OK)
            Process.GetCurrentProcess.Kill()
        End Try
    End Sub

#End Region 'CONSTRUCTOR

#Region "FORM"

    Private Sub Main_Form_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Dim source As HwndSource = HwndSource.FromHwnd(Handle)
        source.AddHook(New HwndSourceHook(AddressOf WndProc))
        Me.Left = My.Settings.WINDOW_MAIN_LEFT
        Me.Top = My.Settings.WINDOW_MAIN_TOP
        AutoSaveTimer = New System.Windows.Forms.Timer
        AutoSaveTimer.Interval = 60000
        AutoSaveTimer.Start()
    End Sub

    Private Sub Main_Form_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        My.Settings.WINDOW_MAIN_LEFT = Me.Left
        My.Settings.WINDOW_MAIN_TOP = Me.Top
        If PLAYER IsNot Nothing Then PLAYER.Dispose()
        If Viewer IsNot Nothing Then
            My.Settings.WINDOW_VIEWER_LEFT = Viewer.Left
            My.Settings.WINDOW_VIEWER_TOP = Viewer.Top
            Viewer.Close()
        End If
        My.Settings.Save()
    End Sub

    Private Sub Main_Form_ContentRendered(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ContentRendered
        AppInit()
    End Sub

    Private Function WndProc(ByVal hwnd As IntPtr, ByVal msg As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr, ByRef handled As Boolean) As IntPtr
        'Debug.WriteLine("WINDOW EVENT: " & msg)
        Select Case msg
            Case EC_USER
                Debug.WriteLine("EC_USER: " & msg)
            Case WM_DVD_EVENT
                'Debug.WriteLine("WM_DVD_EVENT: " & m.Msg)
                If Not PLAYER Is Nothing Then PLAYER.HandleEvent()
        End Select
        Return IntPtr.Zero
    End Function

#End Region 'FORM

#Region "FUNCTIONALITY"

#Region "FUNCTIONALITY:APP"

    Private Sub AppInit()
        Try
            'If Not String.IsNullOrEmpty(m_DVDPath) Then
            '    NewBlankProject()
            '    LoadDVD()
            'Else
            If Not String.IsNullOrEmpty(ProjectPath) Then
                LoadProject(ProjectPath)
            Else
                'load or new project?
                If MessageBox.Show(Me, "Would you like to load a project?", My.Settings.APPLICATION_NAME, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) = MessageBoxResult.Yes Then
                    SelectAndLoadProject()
                Else
                    NewBlankProject()
                    LoadDVD()
                End If
            End If
        Catch ex As Exception
            MsgBox("Problem in AppInit(). Error: " & ex.Message)
            Process.GetCurrentProcess.Kill()
        End Try
    End Sub

    Private Sub ExitRoutiene()
        If PROJECT IsNot Nothing Then OfferSaveProject()
        Application.Current.Shutdown()
    End Sub

#End Region 'FUNCTIONALITY:APP

#Region "FUNCTIONALITY:PROJECT"

    Private WithEvents PROJECT As cBolinas_AuthoringProject

    Private Sub SelectAndLoadProject()
        If PROJECT IsNot Nothing Then OfferSaveProject()
        If SelectProject() Then
            LoadProject(ProjectPath)
        End If
    End Sub

    Private Function SelectProject() As Boolean
        Try
            Dim FSD As New Microsoft.Win32.OpenFileDialog
            FSD.Filter = My.Settings.APPLICATION_NAME_SHORT & " Project|*" & My.Settings.PROJECT_EXT
            FSD.InitialDirectory = My.Settings.LAST_PROJECT_DIR
            FSD.FileName = My.Settings.LAST_PROJECT_NAME
            FSD.Multiselect = False
            If FSD.ShowDialog Then
                If String.IsNullOrEmpty(FSD.FileName) Then
                    GoTo NullProject
                Else
                    ProjectPath = FSD.FileName
                    My.Settings.LAST_PROJECT_DIR = Path.GetDirectoryName(FSD.FileName)
                    My.Settings.Save()
                End If
            Else
NullProject:
                MsgBox("No project selected. Creating new project.")
                ProjectPath = Nothing
                NewBlankProject()
                LoadDVD()
                Return False
            End If
            Return True
        Catch ex As Exception
            MsgBox("Problem with SelectProject(). Error: " & ex.Message)
        End Try
    End Function

    Private Sub LoadProject(ByVal nProjectPath As String)
        PROJECT = cBolinas_AuthoringProject.LoadProjectFromFile(nProjectPath)
        If PROJECT Is Nothing Then
            NewBlankProject()
        Else
            My.Settings.LAST_PROJECT_DIR = Path.GetDirectoryName(nProjectPath)
            My.Settings.LAST_PROJECT_NAME = Path.GetFileName(nProjectPath)
            My.Settings.Save()
            UpdateAllTabsForProject()
            LoadDVD()
        End If
    End Sub

    Private Sub NewBlankProject()
        If PROJECT IsNot Nothing Then
            If Not OfferSaveProject() Then Exit Sub 'user canceled
        End If
        PROJECT = New cBolinas_AuthoringProject

TryAgain:
        If Not PresentProjectSetupDialog() Then
            If MessageBox.Show(Me, "Project setup info must be completed. Try again?", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.YesNo, MessageBoxImage.Exclamation) = MessageBoxResult.Yes Then
                GoTo TryAgain
            Else
                MessageBox.Show(Me, "Project setup failure. Application exiting.", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Stop)
                Process.GetCurrentProcess.Kill()
            End If
        End If

        UpdateAllTabsForProject()

        If String.IsNullOrEmpty(PROJECT.DVDPath) Then
            SelectDVD()
        End If
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns>False on cancel</returns>
    ''' <remarks></remarks>
    Private Function OfferSaveProject() As Boolean
        Dim dr As System.Windows.Forms.DialogResult = MessageBox.Show(Me, "Would you like to save the current project?", My.Settings.APPLICATION_NAME, MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.None)
        Select Case dr
            Case Forms.DialogResult.Yes
                SaveProjectWithDialog()
                Return True
            Case Forms.DialogResult.No
                'do nothing
                Return True
            Case Forms.DialogResult.Cancel
                Return False
        End Select
    End Function

    Private Sub SaveProjectWithDialog()
        Dim SFD As New Microsoft.Win32.SaveFileDialog
        SFD.DefaultExt = My.Settings.PROJECT_EXT
        SFD.Filter = My.Settings.APPLICATION_NAME_SHORT & " Project|*" & My.Settings.PROJECT_EXT
        'SFD.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        SFD.InitialDirectory = Path.GetDirectoryName(ProjectPath)
        SFD.OverwritePrompt = True
        SFD.Title = "Save Project"
        SFD.FileName = PROJECT.Name & My.Settings.PROJECT_EXT
        If SFD.ShowDialog Then
            If Directory.Exists(Path.GetDirectoryName(SFD.FileName)) Then
                ProjectPath = SFD.FileName
                SaveProjectCore()
            End If
        End If
    End Sub

    Private Sub SaveProjectCore(Optional ByVal Silent As Boolean = False)
        If String.IsNullOrEmpty(ProjectPath) Then
            SaveProjectWithDialog()
        Else
            Dim success As Boolean = cBolinas_AuthoringProject.SaveProject(PROJECT, ProjectPath)
            If Not Silent Then
                MsgBox(IIf(success, "Project saved successfully", "Project did not save successfully."))
            End If
        End If
    End Sub

    Private Sub HandleAutoSave(ByVal sender As Object, ByVal e As EventArgs) Handles AutoSaveTimer.Tick
        If PROJECT Is Nothing Then Exit Sub
        SaveProjectCore(True)
    End Sub

#End Region 'FUNCTIONALITY:PROJECT

#Region "FUNCTIONALITY:PLAYBACK"

#Region "FUNCTIONALITY:PLAYBACK:SETUP"

    Private Sub SelectAndLoadDVD()
        SelectDVD()
        LoadDVD()
    End Sub

    Private Sub SelectDVD()
        Try
            If PLAYER IsNot Nothing Then
                PLAYER.Dispose()
                PLAYER = Nothing
            End If
            Dim FSD As New Microsoft.Win32.OpenFileDialog
            FSD.Filter = "DVD (video_ts.ifo)|video_ts.ifo"
            FSD.InitialDirectory = My.Settings.LAST_VIDEOTS_DIR
            FSD.FileName = "video_ts.ifo"
            FSD.Multiselect = False
            If FSD.ShowDialog Then
                If FSD.FileName = "" Then
                    GoTo NullDVD
                Else
                    PROJECT.DVDPath = FSD.FileName
                    My.Settings.LAST_VIDEOTS_DIR = Path.GetDirectoryName(PROJECT.DVDPath)
                    My.Settings.Save()
                End If
            Else
NullDVD:
                If MsgBox("DVD not selected. Would you like to try again?", MsgBoxStyle.YesNo, "Select DVD Path") = MsgBoxResult.Yes Then
                    SelectDVD()
                Else
                    PROJECT.DVDPath = Nothing
                    MessageBox.Show("CRITICAL FAILURE: a DVD path must be selected in order to use this software. Application shutting down.", "FAILED TO SELECT DVD PATH", MessageBoxButton.OK, MessageBoxImage.Stop)
                    Process.GetCurrentProcess.Kill()
                End If
            End If
            UpdateProjectPropertyDisplay()
        Catch ex As Exception
            MsgBox("Problem with SelectDVD(). Error: " & ex.Message)
        End Try
    End Sub

    Private Sub LoadDVD()
ValidateDVDPath:
        If String.IsNullOrEmpty(PROJECT.DVDPath) Then Throw New Exception("Unexpected. DVDPath is empty.")

        ' In case the DVD path has been loaded from the project, we need to verify that the dvd is still available at the path in the project.
        If Not File.Exists(PROJECT.DVDPath) Then
            If MessageBox.Show("The DVD path is not valid. Would you like to reselect a DVD?", "DVD PATH FAILURE", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) = MessageBoxResult.Yes Then
                'prompt for reselect
                PROJECT.DVDPath = ""
                SelectDVD()
                GoTo ValidateDVDPath
            Else
                MessageBox.Show("CRITICAL FAILURE: DVD path was not provided. Application shutting down.", "DVD PATH FAILURE", MessageBoxButton.OK, MessageBoxImage.Stop)
                Process.GetCurrentProcess.Kill()
            End If
        End If
        My.Settings.LAST_VIDEOTS_DIR = Path.GetDirectoryName(PROJECT.DVDPath)
        My.Settings.Save()
        UpdateProjectPropertyDisplay()
        CollectTab_UpdateProjectDisplay()
        If MessageBox.Show(Me, "Start DVD playback?", "DVD", MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.Yes Then
            InitializePlayer()
        End If
    End Sub

#End Region 'FUNCTIONALITY:PLAYBACK:SETUP

#Region "FUNCTIONALITY:PLAYBACK:PLAYER"

    Public WithEvents PLAYER As cDVDPlayer
    Public DVD As cDVD
    Private WithEvents Viewer As Viewer_WPF
    Private ReadOnly Property ViewerAsSMTForm() As cSMTForm
        Get
            If _ViewerAsSMTForm IsNot Nothing Then Return _ViewerAsSMTForm
            _ViewerAsSMTForm = New cSMTForm(Viewer)
            Return _ViewerAsSMTForm
        End Get
    End Property
    Private _ViewerAsSMTForm As cSMTForm

    Public Function InitializePlayer() As Boolean
        Try
            If DVD Is Nothing Then InitializeCDVD()

            'check to see if we've matched regions
            Dim SetNewPlayerRegion As Boolean = False
            Select Case My.Settings.PLAYER_REGION
                Case 0
                    If Not DVD.VMGM.R1 Then SetNewPlayerRegion = True
                Case 1
                    If Not DVD.VMGM.R2 Then SetNewPlayerRegion = True
                Case 2
                    If Not DVD.VMGM.R3 Then SetNewPlayerRegion = True
                Case 3
                    If Not DVD.VMGM.R4 Then SetNewPlayerRegion = True
                Case 4
                    If Not DVD.VMGM.R5 Then SetNewPlayerRegion = True
                Case 5
                    If Not DVD.VMGM.R6 Then SetNewPlayerRegion = True
                Case 6
                    If Not DVD.VMGM.R7 Then SetNewPlayerRegion = True
                Case 7
                    If Not DVD.VMGM.R8 Then SetNewPlayerRegion = True
            End Select

            If SetNewPlayerRegion Then
                'selects the lowest valid region. klumsy but works.
                If DVD.VMGM.R8 Then My.Settings.PLAYER_REGION = 7
                If DVD.VMGM.R7 Then My.Settings.PLAYER_REGION = 6
                If DVD.VMGM.R6 Then My.Settings.PLAYER_REGION = 5
                If DVD.VMGM.R5 Then My.Settings.PLAYER_REGION = 4
                If DVD.VMGM.R4 Then My.Settings.PLAYER_REGION = 3
                If DVD.VMGM.R3 Then My.Settings.PLAYER_REGION = 2
                If DVD.VMGM.R2 Then My.Settings.PLAYER_REGION = 1
                If DVD.VMGM.R1 Then My.Settings.PLAYER_REGION = 0
            End If

            PLAYER = New cDVDPlayer(New cSMTForm(Me), eAVMode.DesktopVMR_NoKeystone, eScalingMode.Native_ScaleToAR, eVideoResolution._720x486)

            Dim ns As sNavigatorSetup
            ns.ASPECT_RATIO = My.Settings.PLAYER_ASPECT_RATIO
            ns.DEFAULT_AUDIO_EXTENSION = SMT.Multimedia.Players.DVD.Enums.eAudioExtensions.Not_Specified
            ns.DEFAULT_AUDIO_LANGUAGE = SMT.Multimedia.Enums.eLanguages.English
            ns.DEFAULT_MENU_LANGUAGE = [Enum].Parse(GetType(SMT.Multimedia.Enums.eLanguages), My.Settings.PLAYER_MENU_LANG)
            ns.DEFAULT_SUBTITLE_EXTENSION = SMT.Multimedia.Players.DVD.Enums.eSubExtensions.Not_Specified
            ns.DEFAULT_SUBTITLE_LANGUAGE = SMT.Multimedia.Enums.eLanguages.English
            ns.PARENTAL_COUNTRY = SMT.Multimedia.Enums.eCountries.UNITED_STATES
            ns.PARENTAL_LEVEL = SMT.Multimedia.Players.DVD.Enums.eParentalLevels.PL_Off
            ns.PLAYER_REGION = My.Settings.PLAYER_REGION
            ns.IsInitalized = True


            'PLAYER.InitializePlayer(PROJECT.DVDPath, ns, Nothing)

            SetupViewer()

            PLAYER.InitializePlayer(PROJECT.DVDPath, ns, ViewerAsSMTForm)

            ShowViewer()

            If DVD.VideoStandard = eVideoStandard.NTSC Then
                'Me.Viewer.Height = 243
                Viewer.ViewerSize = eViewerSize.NTSC_Half_360x243
            Else
                'Me.Viewer.Height = 288
                Viewer.ViewerSize = eViewerSize.PAL_Half_360x288
            End If

            Return True
        Catch ex As Exception
            Me.AddConsoleLine("Problem with InitalizePlayer(). Error: " & ex.Message)
            Return False
        End Try
    End Function

    Private Sub SetupViewer()
        Try
            If Viewer IsNot Nothing Then Viewer.Close()
            Viewer = New Viewer_WPF(PLAYER, My.Settings.APPLICATION_NAME)
            Viewer.Icon = Me.Icon
            Viewer.Left = My.Settings.WINDOW_VIEWER_LEFT
            Viewer.Top = My.Settings.WINDOW_VIEWER_TOP
            Viewer.Topmost = True
            'Viewer.ViewerSize = eViewerSize.HD_Quarter_480x270
        Catch ex As Exception
            Throw New Exception("Problem with SetupViewer(). Error: " & ex.Message, ex)
        End Try
    End Sub

    Private Sub ShowViewer()
        'MsgBox("showviewer")
        'System.Threading.Thread.Sleep(5000)
        Viewer.Show()
    End Sub

    Public Sub InitializeCDVD()
        DVD = New cDVD(PROJECT.DVDPath)
    End Sub

    Private Sub BootDVD()
        Try
            If PLAYER Is Nothing Then Exit Sub
            PLAYER.EjectProject()
            PLAYER.Dispose()
            PLAYER = Nothing
            InitializePlayer()
        Catch ex As Exception
            Me.AddConsoleLine("Problem with BootDVD(). Error: " & ex.Message)
        End Try
    End Sub

#Region "PLAYBACK:PLAYER:EVENT HANDLING"

    Public Sub HandleDomainChanged() Handles PLAYER.evDomainChange
        Try
            EnableAllPositionLablesOnDB()
            If Not PLAYER.PlayState = ePlayState.SystemJP Then
                Dim s As String
                Select Case PLAYER.CurrentDomain
                    Case DvdDomain.FirstPlay
                        s = "First Play"
                        Me.ClearDashboard(True, False)

                    Case DvdDomain.Stop
                        s = "Stop"
                        Me.ClearDashboard(True, False)
                        btnRM_root.IsEnabled = True
                        btnRM_play.IsEnabled = True
                        btnRM_stop.IsEnabled = True

                    Case DvdDomain.Title
                        s = "Title"
                        HandleTitleChanged(-1)

                    Case DvdDomain.VideoManagerMenu
                        s = "VMGM"
                        'SetupSubtitleTab()
                        DisablePositionLablesOnDBForMenuSpace()

                    Case DvdDomain.VideoTitleSetMenu
                        s = "VTSM"
                        'SetupSubtitleTab()
                        DisablePositionLablesOnDBForMenuSpace()
                End Select
            End If
        Catch ex As Exception
            AddConsoleLine("Problem with DomainChanged. Error: " & ex.Message)
        End Try
    End Sub

    Private Sub HandlePlayerInitialized() Handles PLAYER.evPlayerInitialized

        ''ToggleRemote(True)

        'lblVolumes.Text = PLAYER.VolumeCount
        'lblCurrentVol.Text = PLAYER.CurrentVolume
        'lblDiscSide.Text = PLAYER.DiscSide.ToString
        'lblPublisher.Text = PLAYER.CurrentDVD.VMGM.ProviderID
        'lblVTSsTotal.Text = PLAYER.VTSCount

        'txtProjectPath.Text = PLAYER.DVDDirectory

        'Me.btnPS_Eject.Enabled = True
        'Me.btnPS_Reboot.Enabled = True

        ''disc text
        'lblDVDText.Text = PLAYER.DVDText
        'ToolTip.SetToolTip(lblDVDText, lblDVDText.Text)
        'If Not lblDVDText.Text = "Null" And Not lblDVDText.Text = "" Then
        '    'VW.Text = "Phoenix - " & Me.lblDVDText.Text
        'Else
        '    If InStr(PLAYER.DVDDirectory.ToLower, "d:\", CompareMethod.Text) Then
        '        'VW.Text = " " & Me.GetVolumeLabel("D") & " disc in DVD Drive"
        '    End If
        'End If

        'Me.lbParentalManagement.Items.Clear()
        'Dim PMs() As cParentalManagement_US = PLAYER.CurrentDVD.VMGM.GlobalTitleParentalManagement.Titles
        'For Each PM As cParentalManagement_US In PMs
        '    Me.lbParentalManagement.Items.Add(PM.ToString)
        'Next

        ''Me.CurrentUserProfile.AppOptions.Apply()

        '' ADDED 080323 - buil 0.0.8 to hide the 3px top and bottom for NTSC content that keystone adds
        'If Me.PLAYER.CurrentVideoStandard = eVideoStandard.NTSC Then
        '    'Dim trim As Integer = (((Me.pnViewer.Height) / 480) * 3)
        '    'PLAYER.SetViewerPositionInParent(0, 0 - trim, Me.pnViewer.Width, Me.pnViewer.Height)
        '    'Me.pnViewer.Height -= trim
        'End If

    End Sub

    Public Sub HandleSystemJacketPictureDisplayed() Handles PLAYER.evSystemJacketPictureDisplayed
        'Me.btnPS_Reboot.Enabled = False
        'btnPS_Eject.Enabled = False
        'Me.miFILE_Eject.Enabled = False
        'Me.miFILE_Browse.Enabled = True
        'ToggleRemote(False)
        'Me.HideWaitAnimation()
    End Sub

    Private Sub Handle_TileRegionMask_SET(ByVal Value() As Boolean) Handles PLAYER.evRegionMask_SET
        'Reg_1.BackColor = GetBoolColor(Value(0))
        'Reg_2.BackColor = GetBoolColor(Value(1))
        'Reg_3.BackColor = GetBoolColor(Value(2))
        'Reg_4.BackColor = GetBoolColor(Value(3))
        'Reg_5.BackColor = GetBoolColor(Value(4))
        'Reg_6.BackColor = GetBoolColor(Value(5))
        'Reg_7.BackColor = GetBoolColor(Value(6))
        'Reg_8.BackColor = GetBoolColor(Value(7))
    End Sub

    Private Sub Handle_TitleCount_SET(ByVal Value As Short) Handles PLAYER.evGlobalTitleCount_SET
        lblRM_TotalTitles.Text = Value
    End Sub

    Private Sub HandlePlaybackStarted() Handles PLAYER.evPlaybackStarted
        UpdateDashboard()
    End Sub

    Private Sub HandlePlaybackStopped(ByVal Success As Boolean) Handles PLAYER.evPlaybackStopped
        btnRM_stop.Background = System.Windows.Media.Brushes.Gainsboro
        btnRM_pause.Background = System.Windows.Media.Brushes.Gainsboro
        btnRM_fastforward.Background = System.Windows.Media.Brushes.Gainsboro
        btnRM_rewind.Background = System.Windows.Media.Brushes.Gainsboro
        btnRM_stop.Background = System.Windows.Media.Brushes.DarkGray
        btnRM_play.Background = System.Windows.Media.Brushes.Gainsboro
    End Sub

    Private Sub HandlePlaybackPaused(ByVal Paused As Boolean) Handles PLAYER.evPlaybackPaused
        If Paused Then
            btnRM_pause.Background = System.Windows.Media.Brushes.Gainsboro
            btnRM_fastforward.Background = System.Windows.Media.Brushes.Gainsboro
            btnRM_rewind.Background = System.Windows.Media.Brushes.Gainsboro
            btnRM_stop.Background = System.Windows.Media.Brushes.Gainsboro
            btnRM_play.Background = System.Windows.Media.Brushes.DarkGray
        Else
            btnRM_pause.Background = System.Windows.Media.Brushes.DarkGray
            btnRM_fastforward.Background = System.Windows.Media.Brushes.Gainsboro
            btnRM_rewind.Background = System.Windows.Media.Brushes.Gainsboro
            btnRM_stop.Background = System.Windows.Media.Brushes.Gainsboro
            btnRM_play.Background = System.Windows.Media.Brushes.Gainsboro
        End If
        UpdateDashboard()
    End Sub

    Private Sub HandleFastForward(ByVal Rate As Double) Handles PLAYER.evFastForward
        btnRM_pause.Background = System.Windows.Media.Brushes.Gainsboro
        btnRM_fastforward.Background = System.Windows.Media.Brushes.DarkGray
        btnRM_rewind.Background = System.Windows.Media.Brushes.Gainsboro
        btnRM_stop.Background = System.Windows.Media.Brushes.Gainsboro
        btnRM_play.Background = System.Windows.Media.Brushes.Gainsboro
    End Sub

    Private Sub HandleRewind(ByVal Rate As Double) Handles PLAYER.evRewind
        btnRM_pause.Background = System.Windows.Media.Brushes.Gainsboro
        btnRM_fastforward.Background = System.Windows.Media.Brushes.Gainsboro
        btnRM_rewind.Background = System.Windows.Media.Brushes.DarkGray
        btnRM_stop.Background = System.Windows.Media.Brushes.Gainsboro
        btnRM_play.Background = System.Windows.Media.Brushes.Gainsboro
    End Sub

    Private Sub HandleAudioCycled() Handles PLAYER.evAudioCycled
        'Try
        '    For i As Short = 0 To dgAudioStreams.VisibleRowCount - 1
        '        dgAudioStreams.UnSelect(i)
        '    Next
        '    dgAudioStreams.Select(PLAYER.CurrentAudioStreamNumber)
        '    'If Player.CurrentAudioStreamNumber = Player.CurrentAudioStreamCount Then
        '    '    dgAudioStreams.Select(0)
        '    'Else
        '    'End If
        'Catch ex As Exception
        '    'Debug.WriteLine("Could not select current audio stream in the data grid.")
        'End Try
    End Sub

    Private Sub HandleAudioStreamSet(ByVal StreamNumber As Short) Handles PLAYER.evAudioStreamSet
        'lblCurrentAudACMOD.Text = ""
        'lblCurrentAud_Bitrate.Text = ""
        'lblCurrentAudio_ext.Text = ""
        'lblCurrentAudio_format.Text = ""
        'lblCurrentAudio_lang.Text = ""
        'lblCurrentAudio_sn.Text = ""

        'Try
        '    For i As Short = 0 To dgAudioStreams.VisibleRowCount - 1
        '        dgAudioStreams.UnSelect(i)
        '    Next
        '    dgAudioStreams.Select(StreamNumber)
        'Catch ex As Exception
        '    'Debug.WriteLine("Could not select current audio stream in the data grid.")
        'End Try
    End Sub

    Private Sub HandleAudioStreamChanged(ByVal CurrentStream As Byte, ByVal NumberOfStreams As Byte) Handles PLAYER.evAudioStreamChanged
        Try
            If PLAYER.PlayState = ePlayState.SystemJP Then Exit Sub

            If CurrentStream = Nothing Or NumberOfStreams = Nothing Then
                PLAYER.GetAudioStreamStatus(NumberOfStreams, CurrentStream)
            End If

            Dim A As seqAudio = PLAYER.GetAudio(CurrentStream)
            If PLAYER.PlayState = ePlayState.SystemJP Then
                txtRM_CurrentAudio.Text = ""
            Else
                txtRM_CurrentAudio.Text = CurrentStream & " " & A.Language '& " " & A.Extension & " " & A.Format.ToUpper

                'Dim i As Integer = PLAYER.GetAudioBitrate

                'If InStr(A.Format.ToLower, "ac3") Then

                '    i = PLAYER.GetAudioAC3_ACMOD
                '    txtRM_CurrentAudio.Text &= " " & GetAC3ChannelMappingFromAppModeData(i)

                '    i = PLAYER.GetAudioSurroundEncoded

                '    If i = 2 Then
                '        txtRM_CurrentAudio.Text &= " -SUR"
                '    End If
                'ElseIf InStr(A.Format.ToLower, "dts") Then

                'Else

                'End If
            End If

            A = Nothing
        Catch ex As Exception
            If PLAYER.StartingDVD Then Exit Sub
            If PLAYER.PlayState = ePlayState.SystemJP Then Exit Sub
            If InStr(ex.Message, "80040290", CompareMethod.Text) Then Exit Sub
            AddConsoleLine("Problem with HandleAudioStreamChanged(). Error: " & ex.Message)
        End Try
    End Sub

    Private Sub DVDPlaybackRateReturnedToOneX() Handles PLAYER.evDVDPlaybackRateReturnedToOneX
        btnRM_pause.Background = System.Windows.Media.Brushes.Gainsboro
        btnRM_fastforward.Background = System.Windows.Media.Brushes.Gainsboro
        btnRM_rewind.Background = System.Windows.Media.Brushes.Gainsboro
        btnRM_stop.Background = System.Windows.Media.Brushes.Gainsboro
        btnRM_play.Background = System.Windows.Media.Brushes.DarkGray
    End Sub

    Private Sub HandleSubtitleStreamChanged(ByVal NewStreamNumber As Byte) Handles PLAYER.evSubtitleStreamChanged
        Try
            If NewStreamNumber = Nothing Then
                NewStreamNumber = PLAYER.CurrentSubtitleStreamNumber()
            End If

            'For i As Short = 0 To dgSubtitles.VisibleRowCount - 1
            '    dgSubtitles.UnSelect(i)
            'Next

            'If PLAYER.SubtitlesAreOn Then
            '    dgSubtitles.Select(NewStreamNumber)
            'End If

            Debug.WriteLine("SubtitleStreamChanged()")
            Try
                If PLAYER.CurrentDomain = DvdDomain.VideoManagerMenu Or PLAYER.CurrentDomain = DvdDomain.VideoTitleSetMenu Or PLAYER.PlayState = ePlayState.SystemJP Then
                    txtRM_CurrentSubtitle.Text = ""
                    Exit Sub
                End If

                Dim S As seqSub = PLAYER.GetSubtitle(PLAYER.CurrentSubtitleStreamNumber)
                txtRM_CurrentSubtitle.Text = PLAYER.CurrentSubtitleStreamNumber & " " & S.Language '& " " & S.Extension

                'For i As Short = 0 To dgSubtitles.VisibleRowCount - 1
                '    dgSubtitles.UnSelect(i)
                'Next

                'If PLAYER.SubtitlesAreOn Then
                '    lblCurrentSP_onoff.Text = "On"
                '    Me.btnRM_ToggleSubtitles.ButtonStyle = BorderStyles.Office2003
                '    'dgSubtitles.Select(PLAYER.CurrentSubtitleStreamNumber)
                'Else
                '    lblCurrentSP_onoff.Text = "Off"
                '    Me.btnRM_ToggleSubtitles.ButtonStyle = BorderStyles.Default
                'End If

                S = Nothing

            Catch ex As Exception
                If PLAYER.StartingDVD Then Exit Sub
                If PLAYER.PlayState = ePlayState.SystemJP Then Exit Sub
                If InStr(ex.Message, "80040290", CompareMethod.Text) Then Exit Sub
                AddConsoleLine("Problem with SubtitleStreamChanged(). Error: " & ex.Message)
            End Try
        Catch ex As Exception
            'Debug.WriteLine("Could not select current subtitle stream in the data grid.")
        End Try
    End Sub

    Private Sub HandleAngleCycled(ByVal NewAngle As Byte) Handles PLAYER.evAngleCycled
        'do anything? this is handled in angle changed below, no?
    End Sub

    Private Sub HandleClosedCaptionToggle() Handles PLAYER.evClosedCaptionToggle
        If PLAYER.ClosedCaptionsAreOn Then
            Me.btnRM_ToggleClosedCaptions.Background = System.Windows.Media.Brushes.Gainsboro
        Else
            Me.btnRM_ToggleClosedCaptions.Background = System.Windows.Media.Brushes.DarkGray
        End If
    End Sub

    Private Sub HandleRunningTimeTick() Handles PLAYER.evRunningTimeTick
        UpdateDashboard()
        lblRM_TotalChapters.Text = PLAYER.ChaptersInCurrentTitle
        VerifyTab_UpdateDisplay()
    End Sub

    Public Sub HandleChapterChanged(ByVal NewChapterNumber As Byte) Handles PLAYER.evChapterChange
        UpdateDashboard()
        If PLAYER.PlayState = ePlayState.SystemJP Then
            lblRM_CurrentChapter.Text = ""
            lblRM_TotalChapters.Text = ""
        Else
            lblRM_CurrentChapter.Text = NewChapterNumber
            lblRM_TotalChapters.Text = PLAYER.ChaptersInCurrentTitle
        End If
    End Sub

    Public Sub HandleTitleChanged(ByVal NewTitle As Short) Handles PLAYER.evTitleChange
        Try
            If PLAYER.CurrentDomain = DvdDomain.Stop Or PLAYER.CurrentDomain = DvdDomain.FirstPlay Or PLAYER.CurrentDomain = DvdDomain.VideoManagerMenu Or PLAYER.CurrentDomain = DvdDomain.VideoTitleSetMenu Then Exit Sub

            If PLAYER.PlayState = ePlayState.SystemJP Then
                Me.ClearDashboard(False, True)
                Exit Sub
            End If
            Debug.WriteLine("Title Change. TTNo: " & NewTitle)

            'This is needed for when a disc goes into a title for the first time
            'the nav does not throw a titlechanged event when we've not yet been in any title
            'so DomainChanged-Title calls TitleChange with "GetTitle"
            If NewTitle = -1 Then

                'Old way WORKS
                'Dim L As DvdPlayLocation
                'Player.DVDInfo.GetCurrentLocation(L)
                'Debug.WriteLine(L.TitleNum & " - " & Player.CurrentTitle)
                'NewTitle = L.TitleNum

                'new way
                NewTitle = PLAYER.CurrentTitle

                'If NewTitle = 0 Then
                '    PopulateVideoStreamInfoWindow("TitleChange")
                '    Me.lblTotalRunningTime.Text = "0:00:00"
                '    Exit Sub
                'End If
            End If

            Me.lblRM_TotalTitles.Text = PLAYER.GlobalTTCount
            Me.lblRM_CurrentTitle.Text = NewTitle
            Me.lblRM_CurrentChapter.Text = PLAYER.CurrentPlayLocation.ChapterNum
            Me.lblRM_TotalChapters.Text = PLAYER.ChaptersInCurrentTitle

            ''Angles for TT
            'lbAvailAngles.Items.Clear()
            'lblAnglesTotal.Text = PLAYER.CurrentTitleAngleCount
            'lblCurrent_angle.Text = PLAYER.CurrentAngleStream
            'If PLAYER.CurrentTitleAngleCount > 0 Then
            '    For i As Short = 1 To PLAYER.CurrentTitleAngleCount
            '        lbAvailAngles.Items.Add(i)
            '    Next
            'End If

            'Timecode
            CurrentTimecodeDisplayMode = "TTe"

            Dim TC As DvdTimeCode
            Dim M, S, F As String
            If PLAYER.CurrentTitleTRT.bMinutes.ToString.Length = 1 Then
                M = 0 & TC.bMinutes
            Else
                M = TC.bMinutes
            End If

            If PLAYER.CurrentTitleTRT.bSeconds.ToString.Length = 1 Then
                S = 0 & TC.bSeconds
            Else
                S = TC.bSeconds
            End If

            Me.txtRM_TotalRunningTime.Text = PLAYER.CurrentTitleTRT.bHours & ":" & M & ":" & S '& ";" & F

            'lblVTS.Text = PLAYER.CurrentVTS

            'lblVTSTT.Text = PLAYER.CurrentGlobalTT.VTS_TTN
            'Dim VTS As cVTS = PLAYER.CurrentDVD.VTSs(PLAYER.CurrentGlobalTT.VTSN - 1)
            'lblVTSTTsTotal.Text = VTS.TitleCount

            'If PLAYER.CurrentDomain = DvdDomain.Title Then
            '    lblPGCsTotal.Text = VTS.PGCCount_TitleSpace
            'End If

            'Debug.WriteLine("SetupTitleMetaData()")
            If Not PLAYER Is Nothing Then PLAYER.ClearOSD()
            UpdateDashboard()

            ''Subtitles
            ''Dim MI As New MethodInvoker(AddressOf SSI.SetupSubtitleTab)
            ''MI.BeginInvoke(Nothing, Nothing)

            'SetupSubtitleTab()
            Me.HandleSubtitleStreamChanged(PLAYER.CurrentSubtitleStreamNumber)

            'Audio
            'SetupAudioTab()
            Me.HandleAudioStreamChanged(Nothing, Nothing)

            'Video
            'PopulateVideoStreamInfoWindow("TitleChange2")

            CollectTab_UpdateEventVizualizer()

        Catch ex As Exception
            If PLAYER.StartingDVD Then Exit Sub
            If PLAYER.PlayState = ePlayState.SystemJP Then Exit Sub
            'If CheckEx(ex, "Title Change") Then Exit Sub
            AddConsoleLine("Problem with HandleTitleChanged(). Error: " & ex.Message)
        End Try
    End Sub

    Public Sub HandleAngleChanged(ByVal NewAngle As Byte) Handles PLAYER.evAngleChanged
        'PopulateVideoStreamInfoWindow("AngleChanged")
        'lblCurrent_angle.Text = NewAngle
        'ConfigBarDataBurnIn()
        'If NewAngle <> 1 Then MsgBox("need to select the appropriate tab in the video descriptor tabs")
    End Sub

    Public Sub HandleJacketPicturesFound(ByVal JacketPicturePaths() As String) Handles PLAYER.evJacketPicturesFound
        'lbJacketPictures.Items.Clear()
        'For Each s As String In JacketPicturePaths
        '    lbJacketPictures.Items.Add(New cJacketPicture(s))
        'Next
    End Sub

    Public Sub HandleTripleStop() Handles PLAYER.evTripleStop
        'Me.EjectProject()
    End Sub

    Private Sub HandlePlayerConsoleLine(ByVal Type As eConsoleItemType, ByVal Msg As String, ByVal ExtendedInfo As String, ByVal GOPTC As cTimecode) Handles PLAYER.evConsoleLine
        Debug.WriteLine("PLAYER CONSOLE LINE: " & Type.ToString & " " & Msg)
        'Debug.WriteLine(Msg)
        'If Type = eConsoleItemType.WARNING Then
        '    If InStr(Msg, "One of the DVD image files cannot be opened", CompareMethod.Text) Then
        '        Me.AddConsoleLine("Problem reading one or more of the DVD files (IFOs & VOBs).")
        '        Me.CloseProject()
        '    End If
        'End If
    End Sub

    Private Sub HandleWrongRegion(ByVal PlayerRegion As Byte, ByVal ProjectRegions() As Boolean) Handles PLAYER.evWrongRegion
        Debug.WriteLine("guess it is still needed")
        'PLAYER.PlayState = ePlayState.SystemJP 'to prevent the one second timer from trying to update the screen - graph is currently 'broken'
        'Dim dlg As New WrongRegion_Dialog(PlayerRegion, ProjectRegions, Me)
        'If dlg.ShowDialog() = DialogResult.OK Then
        '    Me.BootDVD()
        'Else
        '    Me.CloseProject()
        'End If
    End Sub

    Public Sub HandlePlayerKeyStrikeWPF(ByVal e As KeyEventArgs) Handles PLAYER.evKeyStrikeWPF
        Select Case e.Key
            Case Key.Up
                PLAYER.DirectionalBtnHit(DvdRelButton.Upper)
            Case Key.Down
                PLAYER.DirectionalBtnHit(DvdRelButton.Lower)
            Case Key.Left
                PLAYER.DirectionalBtnHit(DvdRelButton.Left)
            Case Key.Right
                PLAYER.DirectionalBtnHit(DvdRelButton.Right)
            Case Key.Enter
                PLAYER.EnterBtn()
        End Select
    End Sub

    'PLAYER.DirectionalBtnHit(DvdRelButton.Upper)
    'PLAYER.DirectionalBtnHit(DvdRelButton.Lower)

#End Region 'PLAYBACK:PLAYER:EVENT HANDLING

#End Region 'FUNCTIONALITY:PLAYBACK:PLAYER

#Region "FUNCTIONALITY:PLAYBACK:DASHBOARD"

    Public CurrentTimecodeDisplayMode As String = "TTe"

    Private Sub UpdateDashboard()
        Try
            If CurrentTimecodeDisplayMode Is Nothing Then Exit Sub
            If PLAYER.CurrentTitleDuration Is Nothing Then GoTo SkipTCSetup

            Select Case CurrentTimecodeDisplayMode
                Case "TTe" 'Title elapsed
                    Dim M, S, F As String

                    'Total Duration
                    'apply offset to total
                    Dim ct As New cTimecode(PLAYER.CurrentTitleDuration.TotalTime, PLAYER.CurrentVideoStandard = eVideoStandard.NTSC)
                    ct.AddGrossSeconds(PROJECT.EVENTS.RunningTimeOffset)

                    If ct.DVDTimeCode.bMinutes.ToString.Length = 1 Then
                        M = 0 & ct.DVDTimeCode.bMinutes
                    Else
                        M = ct.DVDTimeCode.bMinutes
                    End If

                    If ct.DVDTimeCode.bSeconds.ToString.Length = 1 Then
                        S = 0 & ct.DVDTimeCode.bSeconds
                    Else
                        S = ct.DVDTimeCode.bSeconds
                    End If

                    'If TC.bFrames.ToString.Length = 1 Then
                    '    F = 0 & TC.bFrames
                    'Else
                    '    F = TC.bFrames
                    'End If

                    If Not PLAYER.PlayState = ePlayState.SystemJP Then
                        Me.txtRM_TotalRunningTime.Text = ct.DVDTimeCode.bHours & ":" & M & ":" & S '& ";" & F
                    End If

                    'Current Timecode
                    'apply offset to total
                    ct = New cTimecode(PLAYER.CurrentRunningTime_DVD, PLAYER.CurrentVideoStandard = eVideoStandard.NTSC)
                    ct.AddGrossSeconds(PROJECT.EVENTS.RunningTimeOffset)

                    If ct.DVDTimeCode.bMinutes.ToString.Length = 1 Then
                        M = 0 & ct.DVDTimeCode.bMinutes
                    Else
                        M = ct.DVDTimeCode.bMinutes
                    End If

                    If ct.DVDTimeCode.bSeconds.ToString.Length = 1 Then
                        S = 0 & ct.DVDTimeCode.bSeconds
                    Else
                        S = ct.DVDTimeCode.bSeconds
                    End If

                    'If Player.CurrentRunningTime.bFrames.ToString.Length = 1 Then
                    '    F = 0 & Player.CurrentRunningTime.bFrames
                    'Else
                    '    F = Player.CurrentRunningTime.bFrames
                    'End If

                    Dim ti As String = Microsoft.VisualBasic.Right(ct.DVDTimeCode.bHours.ToString, 1) & ":" & M & ":" & S '& ";" & F

                    If PLAYER.PlayState = ePlayState.Paused Then
                        'ti = "Paused"
                    ElseIf PLAYER.PlayState = ePlayState.Stopped Then
                        ti = "Stopped"
                    End If

                    txtRM_CurrentRunningTime.Text = ti

                Case "TTr" 'Title remaining
                    'CurrentTitleDuration - Player.CurrentRunningTime
                    Dim cur As New cTimecode(PLAYER.CurrentRunningTime_DVD, (PLAYER.CurrentVideoStandard = eVideoStandard.NTSC))
                    Dim tot As New cTimecode(PLAYER.CurrentTitleDuration.TotalTime, (PLAYER.CurrentVideoStandard = eVideoStandard.NTSC))
                    Dim rema As cTimecode = SubtractTimecode(tot, cur, PLAYER.CurrentTargetFrameRate)
                    txtRM_CurrentRunningTime.Text = "Rem: "
                    txtRM_TotalRunningTime.Text = rema.ToString_NoFrames

                Case "CHe" 'Chapter elapased
                Case "CHr" 'Chapter remaining

            End Select

            UpdateActiveSceneDisplay()

SkipTCSetup:
            Me.lblRM_playstate.Text = PLAYER.PlayState.ToString.ToUpper
        Catch ex As Exception
            AddConsoleLine("Problem with UpdateDashboard(). Error: " & ex.Message)
        End Try
    End Sub

    Public Sub ClearDashboard(ByVal DomainChange As Boolean, ByVal FullReset As Boolean)
        Try
            'If Not DomainChange Then txtProjectPath.Text = ""

            txtRM_TotalRunningTime.Text = "0:00:00"
            txtRM_CurrentRunningTime.Text = "0:00:00"
            'lblCurrent_angle.Text = ""
            'lblAnglesTotal.Text = ""
            'lblCurrent_PTT.Text = ""
            'lblPTTsTotal.Text = ""
            'lblEXDB_CurrentTitle.Text = ""
            'lblEXDB_TitleCount.Text = ""
            'lblEXDB_CurrentTitle.Text = ""
            'lblEXDB_TitleCount.Text = ""
            Me.lblRM_TotalChapters.Text = ""
            Me.lblRM_CurrentChapter.Text = ""
            Me.lblRM_CurrentTitle.Text = ""
            Me.lblRM_TotalTitles.Text = ""
            'lblVTS.Text = ""
            'lblVTSsTotal.Text = ""
            'lblVTSTT.Text = ""
            'lblVTSTTsTotal.Text = ""
            'lblPGC.Text = ""
            'lblPGCsTotal.Text = ""
            'lblProgramCur.Text = ""
            'lblProgramsTotal.Text = ""
            'lblCell.Text = ""
            'lblCellsTotal.Text = ""
            'lblCurrentSourceTimecode.Text = "0:00:00"
            'lblCellStillTime.Text = ""

            'Current Subtitle
            'lblCurrentSP_sn.Text = ""
            'lblRM_CurrentSubtitle.Content = ""
            'lblCurrentSP_onoff.Text = ""
            'lblCurrentSP_ext.Text = ""
            txtRM_CurrentSubtitle.Text = ""


            'Current Audio
            'lblCurrentAudACMOD.Text = ""
            'lblCurrentAud_Bitrate.Text = ""
            'lblCurrentAudio_ext.Text = ""
            'lblCurrentAudio_format.Text = ""
            'lblRM_CurrentAudio.Content = ""
            'lblCurrentAudio_sn.Text = ""
            txtRM_CurrentAudio.Text = ""

            ''Volume Info
            'If Not DomainChange Then
            '    lblVolumes.Text = ""
            '    lblPublisher.Text = ""
            '    lblVolumeID.Text = ""
            '    lblCurrentVol.Text = ""
            '    lblDVDText.Text = ""
            '    lblDiscSide.Text = ""
            '    lblCurrentDomain.Text = ""
            '    ToolTip.SetToolTip(lblPublisher, "")
            '    ToolTip.SetToolTip(lblDVDText, "")
            'End If

            ''Video Tab
            'lblVidAtt_aspectratio.Text = ""
            'lblVidAtt_compression.Text = ""
            'lblVidAtt_frameheight.Text = ""
            'lblVidAtt_framerate.Text = ""
            'lblVidAtt_isFilmMode.Text = ""
            'lblVidAtt_VidStd.Text = ""
            'lblVidAtt_isSourceLetterboxed.Text = ""
            'lblVidAtt_lbok.Text = ""
            'lblVidAtt_line21Field1InGOP.Text = ""
            'lblVidAtt_line21Field2InGOP.Text = ""
            'lblVidAtt_psok.Text = ""
            'lblVidAtt_sourceResolution.Text = ""
            ''lblCurrentlyRunning32.Text = ""
            'lblCurrentFrameRate.Text = ""
            'lblCSSEncrypted.Text = ""
            'lblMacrovision.Text = ""
            'lbl32WhichField.Text = ""
            'lblCurrentVideoIsInterlaced.Text = ""
            'lbAvailAngles.Items.Clear()
            ''lblByteRate_Avg.Text = ""
            ''lblByteRate_Cur.Text = ""
            ''lblByteRate_High.Text = ""
            ''lblByteRate_Low.Text = ""
            ''ClearGraph()

            ' ''Audio Tab
            ''Dim Audios As New seqAudioStreams
            ''dgAudioStreams.DataSource = Audios.AudioStreams

            ' ''Subtitle Tab
            ''Dim TempSubs(-1) As seqSub
            ''dgSubtitles.DataSource = TempSubs

            ''Volume Tab
            'If Not DomainChange Then
            '    lbJacketPictures.Items.Clear()
            '    lbAvailableMenuLanguages.Items.Clear()
            '    lbParentalManagement.Items.Clear()
            'End If

            ' ''Params
            ''ClearPRMs()

            ' ''UOPs
            ''ResetUOPs()

            ' ''Remote
            ''lblLBTC.Text = ""
            ''lblLBCh.Text = ""
            ''lblLBTT.Text = ""

            ''If Not DomainChange Then
            ''    cbChapters.Items.Clear()
            ''End If

            If FullReset Then
                'ResetRegions()
                ''cbTitles.Items.Clear()
                btnRM_stop.Background = System.Windows.Media.Brushes.Gainsboro
                btnRM_play.Background = System.Windows.Media.Brushes.Gainsboro
                btnRM_pause.Background = System.Windows.Media.Brushes.Gainsboro
                btnRM_fastforward.Background = System.Windows.Media.Brushes.Gainsboro
                btnRM_rewind.Background = System.Windows.Media.Brushes.Gainsboro
            End If

        Catch ex As Exception
            AddConsoleLine("Problem with ClearDashboard. Error: " & ex.Message & " StackTrace: " & ex.StackTrace)
        End Try
    End Sub

    Sub EnableAllPositionLablesOnDB()
        'Me.lblPGC.Enabled = True
        'Me.lblPGCsTotal.Enabled = True
        'Me.lblVTS.Enabled = True
        'Me.lblVTSsTotal.Enabled = True
        'Me.lblVTSTT.Enabled = True
        'Me.lblVTSTTsTotal.Enabled = True
        'Me.lblProgramCur.Enabled = True
        'Me.lblProgramsTotal.Enabled = True
        'Me.lblCell.Enabled = True
        'Me.lblCellsTotal.Enabled = True
        'Me.lblEXDB_TitleCount.Enabled = True
        'Me.lblEXDB_CurrentTitle.Enabled = True
        Me.lblRM_TotalAngles.IsEnabled = True
        Me.lblRM_CurrentAngle.IsEnabled = True
        'Me.lblCurrent_PTT.Enabled = True
        'Me.lblPTTsTotal.Enabled = True
        'Me.lblEXDB_TitleCount.Enabled = True
        'Me.lblEXDB_CurrentTitle.Enabled = True
        Me.lblRM_TotalTitles.IsEnabled = True
        Me.lblRM_CurrentTitle.IsEnabled = True
    End Sub

    Sub DisablePositionLablesOnDBForMenuSpace()
        'Me.lblVTSTT.Text = ""
        'Me.lblVTSTTsTotal.Text = ""

        'Me.lblEXDB_TitleCount.Text = ""
        'Me.lblEXDB_CurrentTitle.Text = ""

        Me.lblRM_TotalTitles.Text = ""
        Me.lblRM_CurrentTitle.Text = ""

        Me.lblRM_TotalAngles.Text = ""
        Me.lblRM_CurrentAngle.Text = ""

        'Me.lblCurrent_PTT.Text = ""
        'Me.lblPTTsTotal.Text = ""

        Me.lblRM_CurrentChapter.Text = ""
        Me.lblRM_TotalChapters.Text = ""
    End Sub

#End Region 'FUNCTIONALITY:PLAYBACK:DASHBOARD

#End Region 'FUNCTIONALITY:PLAYBACK

#Region "FUNCTIONALITY:TABS"

    Private Sub UpdateAllTabsForProject()
        PrepareTab_UpdateProjectDisplay()
        CollectTab_UpdateProjectDisplay()
    End Sub

#Region "FUNCTIONALITY:TABS:PREPARE"

    Private Sub PromptImportContributors()
        Try
            Dim pth As String = SelectImportFile()
            If String.IsNullOrEmpty(pth) Then
                'MsgBox("No file selected.", MsgBoxStyle.Exclamation)
            Else
                Dim o As New cRCDb_CONTRIBUTOR_COLLECTION(pth)
                If o.Items Is Nothing Then Exit Sub
                PROJECT.CONTRIBUTORS = o
                UpdateContributorsDisplay()
                MsgBox(Path.GetFileName(pth) & " imported successfully.", MsgBoxStyle.Information)
                CollectTab_UpdateCastDisplay()
            End If
        Catch ex As Exception
            AddConsoleLine("Problem with PromptImportContributors(). Error: " & ex.Message)
        End Try
    End Sub

    Private Sub PromptImportFilmography()
        Try
            Dim pth As String = SelectImportFile()
            If String.IsNullOrEmpty(pth) Then
                'MsgBox("No file selected.", MsgBoxStyle.Exclamation)
            Else
                Dim o As New cRCDb_FILMOGRAPHY(pth)
                If o.Items Is Nothing Then Exit Sub
                PROJECT.FILMOGRAPHY = o
                UpdateFilmographyDisplay()
                MsgBox(Path.GetFileName(pth) & " imported successfully.", MsgBoxStyle.Information)
                CollectTab_UpdateCastDisplay()
            End If
        Catch ex As Exception
            AddConsoleLine("Problem with PromptImportFilmography(). Error: " & ex.Message)
        End Try
    End Sub

    Private Sub PromptImportSoundtrack()
        Try
            Dim pth As String = SelectImportFile()
            If String.IsNullOrEmpty(pth) Then
                'MsgBox("No file selected.", MsgBoxStyle.Exclamation)
            Else
                Dim o As New cRCDb_SOUNDTRACK(pth)
                If o.Items Is Nothing Then Exit Sub
                If o.Items(0).WorkId <> PROJECT.Product_ParentWorkId Then
                    If MessageBox.Show("WARNING: The WorkId in the imported soundtrack data does not match the WorkId for the current project. Continue?", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.YesNo, MessageBoxImage.Warning) = MessageBoxResult.No Then
                        Exit Sub
                    End If
                End If
                PROJECT.SOUNDTRACK = o
                UpdateSoundtrackDisplay()
                MsgBox(Path.GetFileName(pth) & " imported successfully.", MsgBoxStyle.Information)
                CollectTab_UpdateSoundtrackDisplay()
            End If
        Catch ex As Exception
            AddConsoleLine("Problem with PromptImportSoundtrack(). Error: " & ex.Message)
        End Try
    End Sub

    Private Sub PromptImportFacts()
        Try
            Dim pth As String = SelectImportFile()
            If String.IsNullOrEmpty(pth) Then
                'MsgBox("No file selected.", MsgBoxStyle.Exclamation)
            Else
                Dim o As New cRCDb_FACTOGRAPHY(pth)
                If o.Items Is Nothing Then Exit Sub
                PROJECT.FACTOGRAPHY = o
                UpdateFactographyDisplay()
                MsgBox(Path.GetFileName(pth) & " imported successfully.", MsgBoxStyle.Information)
                CollectTab_UpdateFactDisplay()
            End If
        Catch ex As Exception
            AddConsoleLine("Problem with PromptImportFacts(). Error: " & ex.Message)
        End Try
    End Sub

    Private Sub PrepareTab_UpdateProjectDisplay()
        UpdateProjectPropertyDisplay()
        UpdateContributorsDisplay()
        UpdateFactographyDisplay()
        UpdateFilmographyDisplay()
        UpdateSoundtrackDisplay()
    End Sub

    Private Sub UpdateProjectPropertyDisplay()


        '===================
        ' PROGRESS BARS
        '===================
        Me.pbPreparation.Value = PROJECT.Progress_Prepare
        Me.pbCollect.Value = PROJECT.Progress_Collect
        Me.pbVerify.Value = PROJECT.Progress_Verify


        '===================
        ' PREPARATION
        '===================
        Me.lblProjectName.Content = PROJECT.Name
        Me.lblDVDPath.Content = PROJECT.DVDPath

        Me.lblPRE_GN_ProjectName.Content = PROJECT.Name
        Me.lblPRE_GN_DVDPath.Content = PROJECT.DVDPath
        Me.lblPRE_GN_ProductId.Content = PROJECT.Product_Id
        Me.lblPRE_GN_ProductTitle.Content = PROJECT.Product_Title
        Me.lblPRE_GN_ProductReleased.Content = PROJECT.Product_ReleasedDate
        Me.lblPRE_GN_ProductStudio.Content = PROJECT.Product_Studio
        Me.lblPRE_GN_ProductType.Content = PROJECT.Product_Type.ToString
        Me.lblPRE_GN_ProductLang.Content = PROJECT.Product_Lang.ToString
        Me.lblPRE_GN_ParentWorkId.Content = PROJECT.Product_ParentWorkId
        Me.lblPRE_GN_DVDTitleNumber.Content = PROJECT.EVENTS.DVDTitleNumber
        Me.lblPRE_GN_DVDTitleDuration.Content = PROJECT.EVENTS.DVDTitleDuration
        Me.lblPRE_GN_RunningTimeOffset.Content = PROJECT.EVENTS.RunningTimeOffset

        If PROJECT.CONTRIBUTORS IsNot Nothing AndAlso PROJECT.CONTRIBUTORS.Items.Count > 0 Then
            lblOV_PR_CO_Imported.Content = "True"
            lblOV_PR_CO_Date.Content = PROJECT.CONTRIBUTORS.ImportDateUTC.ToLocalTime.ToString
            lblOV_PR_CO_Count.Content = PROJECT.CONTRIBUTORS.Items.Count
            pbPR_Contributors.Value = 100
        Else
            lblOV_PR_CO_Imported.Content = "False"
            lblOV_PR_CO_Date.Content = ""
            lblOV_PR_CO_Count.Content = ""
            pbPR_Contributors.Value = 0
        End If

        If PROJECT.FILMOGRAPHY IsNot Nothing AndAlso PROJECT.FILMOGRAPHY.Items.Count > 0 Then
            lblOV_PR_FO_Imported.Content = "True"
            lblOV_PR_FO_Date.Content = PROJECT.FILMOGRAPHY.ImportDateUTC.ToLocalTime.ToString
            lblOV_PR_FO_Count.Content = PROJECT.FILMOGRAPHY.Items.Count
            pbPR_Filmography.Value = 100
        Else
            lblOV_PR_FO_Imported.Content = "False"
            lblOV_PR_FO_Date.Content = ""
            lblOV_PR_FO_Count.Content = ""
            pbPR_Filmography.Value = 0
        End If

        If PROJECT.SOUNDTRACK IsNot Nothing AndAlso PROJECT.SOUNDTRACK.Items.Count > 0 Then
            lblOV_PR_ST_Imported.Content = "True"
            lblOV_PR_ST_Date.Content = PROJECT.SOUNDTRACK.ImportDateUTC.ToLocalTime.ToString
            lblOV_PR_ST_Count.Content = PROJECT.SOUNDTRACK.Items.Count
            pbPR_Soundtrack.Value = 100
        Else
            lblOV_PR_ST_Imported.Content = "False"
            lblOV_PR_ST_Date.Content = ""
            lblOV_PR_ST_Count.Content = ""
            pbPR_Soundtrack.Value = 0
        End If

        If PROJECT.FACTOGRAPHY IsNot Nothing AndAlso PROJECT.FACTOGRAPHY.Items.Count > 0 Then
            lblOV_PR_MF_Imported.Content = "True"
            lblOV_PR_MF_Date.Content = PROJECT.FACTOGRAPHY.ImportDateUTC.ToLocalTime.ToString
            lblOV_PR_MF_Count.Content = PROJECT.FACTOGRAPHY.Items.Count
            pbPR_Facts.Value = 100
        Else
            lblOV_PR_MF_Imported.Content = "False"
            lblOV_PR_MF_Date.Content = ""
            lblOV_PR_MF_Count.Content = ""
            pbPR_Facts.Value = 0
        End If


        '===================
        ' COLLECTION
        '===================
        lblOV_CO_SceneCount.Content = PROJECT.EVENTS.Scenes.Items.Count
        lblOV_CO_SceneCoveragePer.Content = PROJECT.EVENTS.SceneCoverage_Percent & "%"
        lblOV_CO_SceneCoverageMin.Content = PROJECT.EVENTS.SceneCoverage_Minutes & " min"
        lblOV_CO_CastCount.Content = PROJECT.EVENTS.CastCount
        lblOV_CO_CastCoverage.Content = "N/A"
        lblOV_CO_TrackCount.Content = PROJECT.EVENTS.MusicTrackCount
        lblOV_CO_TrackCoverage.Content = "N/A"
        lblOV_CO_FactCount.Content = PROJECT.EVENTS.FactCount
        lblOV_CO_FactCoverage.Content = "N/A"


        '===================
        ' VERIFICATION
        '===================


    End Sub

    Private Sub UpdateContributorsDisplay()
        lvPR_Contributors.ItemsSource = Nothing
        lvPR_Contributors.ItemsSource = PROJECT.CONTRIBUTORS.Items
        UpdateProjectPropertyDisplay()
    End Sub

    Private Sub UpdateFilmographyDisplay()
        lvPR_Filmography.ItemsSource = Nothing
        lvPR_Filmography.ItemsSource = PROJECT.FILMOGRAPHY.Items
        UpdateProjectPropertyDisplay()
        CollectTab_UpdateCastDisplay()
    End Sub

    Private Sub UpdateSoundtrackDisplay()
        lvPR_Soundtrack.ItemsSource = Nothing
        lvPR_Soundtrack.ItemsSource = PROJECT.SOUNDTRACK.Items
        UpdateProjectPropertyDisplay()
    End Sub

    Private Sub UpdateFactographyDisplay()
        lvPR_Facts.ItemsSource = Nothing
        lvPR_Facts.ItemsSource = PROJECT.FACTOGRAPHY.Items
        UpdateProjectPropertyDisplay()
    End Sub

    Private Sub ClearContributors()
        If MsgBox("Clear contributors?", MsgBoxStyle.YesNo) = MsgBoxResult.No Then Exit Sub
        PROJECT.CONTRIBUTORS = New cRCDb_CONTRIBUTOR_COLLECTION()
        UpdateContributorsDisplay()
    End Sub

    Private Sub ClearFilmography()
        If MsgBox("Clear filmography?", MsgBoxStyle.YesNo) = MsgBoxResult.No Then Exit Sub
        PROJECT.FILMOGRAPHY = New cRCDb_FILMOGRAPHY()
        UpdateFilmographyDisplay()
    End Sub

    Private Sub ClearSoundtrack()
        If MsgBox("Clear soundtrack?", MsgBoxStyle.YesNo) = MsgBoxResult.No Then Exit Sub
        PROJECT.SOUNDTRACK = New cRCDb_SOUNDTRACK()
        UpdateSoundtrackDisplay()
    End Sub

    Private Sub ClearFacts()
        If MsgBox("Clear facts?", MsgBoxStyle.YesNo) = MsgBoxResult.No Then Exit Sub
        PROJECT.FACTOGRAPHY = New cRCDb_FACTOGRAPHY()
        UpdateFactographyDisplay()
    End Sub

    Private Sub ShowContributorItemEditor(ByRef CI As cRCDb_CONTRIBUTOR_ITEM)
        Dim dlg As New ContributorItemEditor_Dialog
        dlg.Owner = Me
        If CI IsNot Nothing Then
            dlg.ContributorName = CI.ContributorName
            dlg.ReferenceId = CI.ContributorId
        End If
        Dim b As Boolean = dlg.ShowDialog
        If b Then
            If CI Is Nothing Then
                CI = New cRCDb_CONTRIBUTOR_ITEM(dlg.ReferenceId, dlg.ContributorName)
                PROJECT.CONTRIBUTORS.Items.Add(CI)
            Else
                CI.ContributorId = dlg.ReferenceId
                CI.ContributorName = dlg.ContributorName
            End If
            UpdateContributorsDisplay()
        End If
    End Sub


    Private Sub ShowFilmographyItemEditor(ByRef FI As cRCDb_FILMOGRAPHY_ITEM)
        Dim dlg As New FilmographyItemEditor_Dialog
        dlg.Owner = Me
        If FI IsNot Nothing Then
            dlg.Id = FI.Id
            dlg.ContributorId = FI.ContributorId
            dlg.CharacterName = FI.CharacterName
            dlg.Role = FI.Role
            dlg.WorkId = FI.WorkId
            dlg.WorkDate = FI.WorkDate
            dlg.WorkStudio = FI.WorkStudio
        End If
        Dim b As Boolean = dlg.ShowDialog
        If b Then
            If FI Is Nothing Then
                FI = New cRCDb_FILMOGRAPHY_ITEM()
                FI.Id = dlg.Id
                FI.ContributorId = dlg.ContributorId
                FI.CharacterName = dlg.CharacterName
                FI.Role = dlg.Role
                FI.WorkId = dlg.WorkId
                FI.WorkDate = dlg.WorkDate
                FI.WorkStudio = dlg.WorkStudio
                PROJECT.FILMOGRAPHY.Items.Add(FI)
            Else
                FI.Id = dlg.Id
                FI.ContributorId = dlg.ContributorId
                FI.CharacterName = dlg.CharacterName
                FI.Role = dlg.Role
                FI.WorkId = dlg.WorkId
                FI.WorkDate = dlg.WorkDate
                FI.WorkStudio = dlg.WorkStudio
            End If
            UpdateFilmographyDisplay()
            CollectTab_UpdateCastDisplay()
        End If
    End Sub

    Public Sub ShowSoundtrackItemEditor(ByRef SI As cRCDb_MUSIC_ITEM)
        Dim dlg As New SoundtrackItemEditor_Dialog
        dlg.Owner = Me
        If SI IsNot Nothing Then
            dlg.Id = SI.Id
            dlg.Album = SI.Album
            dlg.Artist = SI.Artist
            dlg.Time = SI.Time
            dlg.Label = SI.Label
            dlg.ReleaseDate = SI.ReleaseDate
            dlg.ReferenceId = SI.ReferenceId
            dlg.Title = SI.Title
            dlg.WorkId = SI.WorkId
            dlg.Notes = SI.Notes
        End If
        Dim b As Boolean = dlg.ShowDialog
        If b Then
            If SI Is Nothing Then
                SI = New cRCDb_MUSIC_ITEM()
                SI.Id = dlg.Id
                SI.Album = dlg.Album
                SI.Artist = dlg.Artist
                SI.Label = dlg.Label
                SI.Notes = dlg.Notes
                SI.ReferenceId = dlg.ReferenceId
                SI.ReleaseDate = dlg.ReleaseDate
                SI.Time = dlg.Time
                SI.Title = dlg.Title
                SI.WorkId = dlg.WorkId
                PROJECT.SOUNDTRACK.Items.Add(SI)
            Else
                SI.Id = dlg.Id
                SI.Album = dlg.Album
                SI.Artist = dlg.Artist
                SI.Label = dlg.Label
                SI.Notes = dlg.Notes
                SI.ReferenceId = dlg.ReferenceId
                SI.ReleaseDate = dlg.ReleaseDate
                SI.Time = dlg.Time
                SI.Title = dlg.Title
                SI.WorkId = dlg.WorkId
            End If
            UpdateSoundtrackDisplay()
            CollectTab_UpdateSoundtrackDisplay()
        End If
    End Sub

    Public Sub ShowFactItemEditor(ByRef FI As cRCDb_FACT_ITEM)
        Dim dlg As New FactItemEditor_Dialog
        dlg.Owner = Me
        If FI IsNot Nothing Then
            dlg.Id = FI.Id
            dlg.FactReference = FI.FactReference
            dlg.Description = FI.Description
            dlg.Generic = FI.Generic
            dlg.FactRefType = FI.FactReferenceType
            dlg.FactRefId = FI.FactReferenceId
            dlg.Source1 = FI.Source1
            dlg.Source2 = FI.Source2
            dlg.Source3 = FI.Source3
        End If
        Dim b As Boolean = dlg.ShowDialog
        If b Then
            If FI Is Nothing Then
                FI = New cRCDb_FACT_ITEM()
                FI.Id = dlg.Id
                FI.Description = dlg.Description
                FI.FactReference = dlg.FactReference
                FI.FactReferenceId = dlg.FactRefId
                FI.FactReferenceType = dlg.FactRefType
                FI.Generic = dlg.Generic
                FI.Source1 = dlg.Source1
                FI.Source2 = dlg.Source2
                FI.Source3 = dlg.Source3
                PROJECT.FACTOGRAPHY.Items.Add(FI)
            Else
                FI.Id = dlg.Id
                FI.Description = dlg.Description
                FI.FactReference = dlg.FactReference
                FI.FactReferenceId = dlg.FactRefId
                FI.FactReferenceType = dlg.FactRefType
                FI.Generic = dlg.Generic
                FI.Source1 = dlg.Source1
                FI.Source2 = dlg.Source2
                FI.Source3 = dlg.Source3
            End If
            UpdateFactographyDisplay()
            CollectTab_UpdateFactDisplay()
        End If
    End Sub

    Private Sub PRE_CON_DeleteSelected()
        PROJECT.CONTRIBUTORS.Items.Remove(lvPR_Contributors.SelectedItem)
        UpdateContributorsDisplay()
    End Sub

    Private Sub PRE_FLM_DeleteSelected()
        PROJECT.FILMOGRAPHY.Items.Remove(lvPR_Filmography.SelectedItem)
        UpdateFilmographyDisplay()
    End Sub

    Private Sub PRE_ST_DeleteSelected()
        PROJECT.SOUNDTRACK.Items.Remove(lvPR_Soundtrack.SelectedItem)
        UpdateSoundtrackDisplay()
    End Sub

    Private Sub PRE_FT_DeleteSelected()
        PROJECT.FACTOGRAPHY.Items.Remove(lvPR_Facts.SelectedItem)
        UpdateFactographyDisplay()
    End Sub

#End Region 'FUNCTIONALITY:TABS:PREPARE

#Region "FUNCTIONALITY:TABS:COLLECT"

    Private Sub CollectTab_UpdateProjectDisplay()
        CollectTab_UpdateEventVizualizer()
        CollectTab_UpdateSceneDisplay()
        CollectTab_UpdateCastDisplay()
        CollectTab_UpdateSoundtrackDisplay()
        CollectTab_UpdateFactDisplay()
    End Sub

    Private Sub CollectTab_UpdateEventVizualizer()
        EventVizualizer.Setup(PLAYER, PROJECT.EVENTS)
    End Sub

    Private Sub CollectTab_UpdateSceneDisplay()
        ''DEBUGGING - DUMMY DATA
        'For i As Integer = 50 To 1 Step -1
        '    PROJECT.SCENES.Items.Add(New cRCDb_SCENE_ITEM(PROJECT.SCENES, i * 10, i * 10, i))
        'Next
        ''DEBUGGING - DUMMY DATA
        Me.lvCO_SC_Scenes.ItemsSource = Nothing
        Me.lvCO_SC_Scenes.ItemsSource = PROJECT.SCENES.Items

        ''debugging
        'For Each s As cRCDb_SCENE_ITEM In PROJECT.EVENTS.Scenes.ItemsSorted
        '    Debug.WriteLine(s.Number & " s = " & s.StartTime & "  e = " & s.EndTime)
        'Next
        ''debugging
    End Sub

    Private Sub CollectTab_UpdateCastDisplay()
        Dim c As List(Of cRCDb_CONTRIBUTOR_ITEM) = PROJECT.GetCast
        If c Is Nothing Then Exit Sub
        If c.Count = 0 And PROJECT.CONTRIBUTORS.Items.Count > 0 Then
            MessageBox.Show("WARNING: It seems that the imported filmography/contributors data does not match the current project's WorkId.", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Warning)
        Else
            Me.lvCO_CS_Cast.ItemsSource = Nothing
            Me.lvCO_CS_Cast.ItemsSource = PROJECT.GetCast
        End If
    End Sub

    Private Sub CollectTab_UpdateSoundtrackDisplay()
        Me.lvCO_ST_Soundtrack.ItemsSource = Nothing
        Me.lvCO_ST_Soundtrack.ItemsSource = PROJECT.SOUNDTRACK.Items
    End Sub

    Private Sub CollectTab_UpdateFactDisplay()
        Me.lvCO_FC_Facts.ItemsSource = Nothing
        Me.lvCO_FC_Facts.ItemsSource = PROJECT.FACTOGRAPHY.Items
    End Sub

    Private Sub Handle_EventVisualizer_ProgressUpdate() Handles EventVizualizer.evProgressUpdate
        UpdateProjectPropertyDisplay()
    End Sub

#Region "FUNCTIONALITY:TABS:COLLECT:SCENES"

    Private Sub NewScene()
        Try
            Dim n As String = InputBox("Scene Name:", My.Settings.APPLICATION_NAME_SHORT)
            If n = "" Then
                MessageBox.Show("New Scene Canceled.", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Exit Sub
            End If
            Dim ns As New cRCDb_SCENE_ITEM(n, "")
            PROJECT.SCENES.Items.Add(ns)

            If PLAYER Is Nothing Then Exit Sub
            Dim s, e As Integer

            If PLAYER.CurrentDomain <> DvdDomain.Title Then
                MsgBox("Scenes cannot be created outside of DVD title space.")
                Exit Sub
            End If
            s = PLAYER.CurrentRunningTime_InSeconds
            e = PROJECT.EVENTS.Scenes.GetEndTime(s, PROJECT.EVENTS.DVDTitleDuration)


            'If PROJECT.EVENTS.Scenes.EventTimesOverlap(s, e) Then
            '    MessageBox.Show("A scene already exists at this point.", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Exclamation)
            '    PROJECT.SCENES.Items.Remove(ns)
            '    Exit Sub
            'End If
            Dim ev As New cRCDb_EVENT_ITEM("", s, e)
            ev.SCENE = ns
            PROJECT.EVENTS.Scenes.Items.Add(ev)
            CollectTab_UpdateEventVizualizer()
            CollectTab_UpdateSceneDisplay()
        Catch ex As Exception
            Me.AddConsoleLine("Problem with NewScene(). Error: " & ex.Message)
        End Try
    End Sub

    Private ReadOnly Property ActiveSceneIndex() As Integer
        Get
            'If ActiveScene Is Nothing Then Return Nothing
            'Return PROJECT.SCENES.Items.IndexOf(ActiveScene)
        End Get
    End Property

    Private ReadOnly Property ActiveScene() As cRCDb_SCENE_ITEM
        Get
            'If PROJECT Is Nothing Then Return Nothing
            'If PLAYER Is Nothing Then Return Nothing
            'If PROJECT.SCENES Is Nothing Then Return Nothing
            'Return PROJECT.SCENES.FindScene(PLAYER.CurrentRunningTime_InSeconds)
        End Get
    End Property

    Private Sub UpdateActiveSceneDisplay()
        'Dim ac As cRCDb_SCENE_ITEM = ActiveScene
        'If ac Is Nothing Then Exit Sub
        'Me.lblCO_SC_ActiveSceneId.Content = ac.Name
        'Me.lblCO_SC_ActiveSceneStartTime.Content = New cTimecode(ac.StartTime, eFramerate.NTSC).ToString_NoFrames
        'Me.lblCO_SC_ActiveSceneEndTime.Content = New cTimecode(ac.EndTime, eFramerate.NTSC).ToString_NoFrames
    End Sub

    Private Sub DeleteSelectedScene()
        If lvCO_SC_Scenes.SelectedItem Is Nothing Then Exit Sub
        Dim S As cRCDb_SCENE_ITEM = CType(lvCO_SC_Scenes.SelectedItem, cRCDb_SCENE_ITEM)
        If MessageBox.Show("Delect selected scene?" & vbNewLine & vbNewLine & S.Name, My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.No Then Exit Sub
        PROJECT.SCENES.Items.Remove(S)
        CollectTab_UpdateSceneDisplay()
    End Sub

#End Region 'FUNCTIONALITY:TABS:COLLECT:SCENES

#Region "FUNCTIONALITY:TABS:COLLECT:CAST"

    Private Sub CollectTab_AddCurrentActor()
        'If lvCO_CS_ActiveCast.Items.IndexOf(lvCO_CS_Cast.SelectedItem) = -1 Then
        '    lvCO_CS_ActiveCast.Items.Add(lvCO_CS_Cast.SelectedItem)
        '    ActiveScene.ACTIVE_CAST.Add(CType(lvCO_CS_Cast.SelectedItem, cBolinas_CastItem).ToContributorItem)
        'Else
        '    'remove it?
        'End If
    End Sub

    Private Sub CollectTab_RemoveCurrentActor()
        'lvCO_CS_ActiveCast.Items.Remove(Me.lvCO_CS_ActiveCast.SelectedItem)
        'ActiveScene.ACTIVE_CAST.Remove(CType(lvCO_CS_Cast.SelectedItem, cBolinas_CastItem).ToContributorItem)
    End Sub

#End Region 'FUNCTIONALITY:TABS:COLLECT:CAST

#Region "FUNCTIONALITY:TABS:COLLECT:SOUNDTRACK"

    Private Sub CollectTab_AddCurrentMusicTrack()
        'ActiveScene.ACTIVE_SOUNDTRACK.Add(Me.lvCO_ST_Soundtrack.SelectedItem)
        'lblCO_ST_ActiveTrack.Content = CType(Me.lvCO_ST_Soundtrack.SelectedItem, cRCDb_MUSIC_ITEM).Title
    End Sub

#End Region 'FUNCTIONALITY:TABS:COLLECT:SOUNDTRACK

#Region "FUNCTIONALITY:TABS:COLLECT:FACTS"

    Dim _lastHeaderClicked As GridViewColumnHeader = Nothing
    Dim _lastDirection As ListSortDirection = ListSortDirection.Ascending

    Private Sub HandleFactRefTypeHeaderClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim headerClicked As GridViewColumnHeader = TryCast(e.OriginalSource, GridViewColumnHeader)
        Dim direction As ListSortDirection

        If headerClicked IsNot Nothing Then
            If headerClicked.Role <> GridViewColumnHeaderRole.Padding Then
                If headerClicked IsNot _lastHeaderClicked Then
                    direction = ListSortDirection.Ascending
                Else
                    If _lastDirection = ListSortDirection.Ascending Then
                        direction = ListSortDirection.Descending
                    Else
                        direction = ListSortDirection.Ascending
                    End If
                End If

                'Dim header As String = TryCast(headerClicked.Column.Header, String)
                FactColumnSort("FactReferenceType", direction)

                'If direction = ListSortDirection.Ascending Then
                '    headerClicked.Column.HeaderTemplate = TryCast(Resources("HeaderTemplateArrowUp"), DataTemplate)
                'Else
                '    headerClicked.Column.HeaderTemplate = TryCast(Resources("HeaderTemplateArrowDown"), DataTemplate)
                'End If

                ' Remove arrow from previously sorted header
                If _lastHeaderClicked IsNot Nothing AndAlso _lastHeaderClicked IsNot headerClicked Then
                    _lastHeaderClicked.Column.HeaderTemplate = Nothing
                End If

                _lastHeaderClicked = headerClicked
                _lastDirection = direction
            End If
        End If
    End Sub

    Private Sub FactColumnSort(ByVal sortBy As String, ByVal direction As ListSortDirection)
        Dim dataView As ICollectionView = CollectionViewSource.GetDefaultView(lvCO_FC_Facts.ItemsSource)
        dataView.SortDescriptions.Clear()
        Dim sd As New SortDescription(sortBy, direction)
        dataView.SortDescriptions.Add(sd)
        dataView.Refresh()
    End Sub

#End Region 'FUNCTIONALITY:TABS:COLLECT:FACTS

#End Region 'FUNCTIONALITY:TABS:COLLECT

#Region "FUNCTIONALITY:TABS:VERIFY"

    Private VerifyTab_CurrentSceneItem As cRCDb_SCENE_ITEM
    Private VerifyTab_CurrentMusicItem As cRCDb_MUSIC_ITEM

    Private Sub VerifyTab_UpdateDisplay()
        Try
            VerifyTab_ClearData()

            If PLAYER Is Nothing Then Exit Sub
            If PLAYER.PlayState <> ePlayState.Playing Then Exit Sub
            If PLAYER.CurrentDomain <> DvdDomain.Title Then Exit Sub
            If PLAYER.CurrentTitle <> PROJECT.EVENTS.DVDTitleNumber Then Exit Sub

            Dim EVs As List(Of cRCDb_EVENT_ITEM) = PROJECT.EVENTS.GetActiveEvents(PLAYER.CurrentRunningTime_InSeconds)
            For Each EV As cRCDb_EVENT_ITEM In EVs

                'SCENE DATA
                If EV.SCENE IsNot Nothing AndAlso EV.SCENE.IsInitalized Then
                    Me.lblVF_SC_ActiveSceneId.Content = EV.SCENE.Id
                    Me.lblVF_SC_StartTime.Content = EV.Start
                    Me.lblVF_SC_EndTime.Content = EV.End
                    Me.lblVF_SC_Name.Content = EV.SCENE.Name
                    Me.txtVF_SC_Description.Text = EV.SCENE.Description
                    VerifyTab_CurrentSceneItem = EV.SCENE
                End If

                'CAST DATA
                For Each c As cRCDb_CONTRIBUTOR_ITEM In EV.CAST
                    Me.lvVF_CS_Cast.Items.Add(c)
                Next

                'SOUNDTRACK DATA
                If EV.SOUNDTRACK.IsInitalized Then
                    Me.lblVF_ST_ActiveTrack.Content = EV.SOUNDTRACK.Title
                    VerifyTab_CurrentMusicItem = EV.SOUNDTRACK
                End If

                'FACT DATA
                For Each f As cRCDb_FACT_ITEM In EV.FACTS
                    Me.lvVF_FC_Facts.Items.Add(f)
                Next

            Next
        Catch ex As Exception
            Me.AddConsoleLine("Problem with VerifyTab_UpdateDisplay(). Error: " & ex.Message)
        End Try
    End Sub

    Private Sub VerifyTab_ClearData()
        Me.lblVF_SC_ActiveSceneId.Content = ""
        Me.lblVF_SC_StartTime.Content = ""
        Me.lblVF_SC_EndTime.Content = ""
        Me.lblVF_SC_Name.Content = ""
        Me.txtVF_SC_Description.Text = ""

        Me.lvVF_CS_Cast.Items.Clear()
        Me.lblVF_ST_ActiveTrack.Content = ""
        Me.lvVF_FC_Facts.Items.Clear()
    End Sub

#Region "FUNCTIONALITY:TABS:VERIFY:CONTEXT MENU"

    Private WithEvents VerifyTab_ItemContextMenu As ContextMenu

    Private Sub ShowVerifyTabItemContextMenu(ByRef lv As ListView, ByRef p As System.Windows.Point)
        Try
            VerifyTab_ItemContextMenu = New System.Windows.Controls.ContextMenu
            VerifyTab_ItemContextMenu.HasDropShadow = True
            VerifyTab_ItemContextMenu.Placement = Primitives.PlacementMode.Relative
            VerifyTab_ItemContextMenu.HorizontalOffset = p.X
            VerifyTab_ItemContextMenu.VerticalOffset = p.Y
            VerifyTab_ItemContextMenu.PlacementTarget = lv
            VerifyTab_ItemContextMenu.Tag = lv.SelectedItem

            Dim tMI As System.Windows.Controls.MenuItem
            Dim sep As System.Windows.Controls.Separator

            tMI = New System.Windows.Controls.MenuItem()
            tMI.Header = "Remove"
            tMI.IsEnabled = True
            AddHandler tMI.Click, AddressOf Handle_ItemContextMenu_EventRemove
            VerifyTab_ItemContextMenu.Items.Add(tMI)

            tMI = New System.Windows.Controls.MenuItem()
            tMI.Header = "Edit"
            tMI.IsEnabled = True
            AddHandler tMI.Click, AddressOf Handle_ItemContextMenu_ShowEditor
            VerifyTab_ItemContextMenu.Items.Add(tMI)

            'sep = New System.Windows.Controls.Separator
            'sep.SnapsToDevicePixels = True
            'sep.Height = 1
            'StreamContextMenu.Items.Add(sep)

            VerifyTab_ItemContextMenu.IsOpen = True

        Catch ex As Exception
            Throw New Exception("Problem with ShowItemContextMenu(). Error: " & ex.Message, ex)
        End Try
    End Sub

    Private Sub Handle_ItemContextMenu_EventRemove(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        'Dim n As String = VerifyTab_ItemContextMenu.Tag.GetType.Name
        'If InStr(n.ToLower, "contributor") Then
        '    Dim c As cRCDb_CONTRIBUTOR_ITEM = VerifyTab_ItemContextMenu.Tag
        '    If MessageBox.Show("Remove " & c.ContributorName & " from event?", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.No Then Exit Sub
        '    EI.CAST.Remove(c)
        'End If
        'If InStr(n.ToLower, "fact") Then
        '    Dim f As cRCDb_FACT_ITEM = VerifyTab_ItemContextMenu.Tag
        '    If MessageBox.Show("Remove fact (" & f.Id & ") from event?", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.No Then Exit Sub
        '    EI.FACTS.Remove(f)
        'End If
        'PROJECT.EVENTS.ForceRefresh()
        MsgBox("nop.")
    End Sub

    Private Sub Handle_ItemContextMenu_ShowEditor(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim n As String = VerifyTab_ItemContextMenu.Tag.GetType.Name
        If InStr(n.ToLower, "contributor") Then
            ShowCastItemEditor(VerifyTab_ItemContextMenu.Tag)
        End If
        If InStr(n.ToLower, "fact") Then
            ShowFactEditor(VerifyTab_ItemContextMenu.Tag)
        End If
    End Sub

#End Region 'FUNCTIONALITY:TABS:VERIFY:CONTEXT MENU

#End Region 'FUNCTIONALITY:TABS:VERIFY

#End Region 'FUNCTIONALITY:TABS

#Region "FUNCTIONALITY:DIALOGS"

    Private Function SelectImportFile() As String
        Try
            Dim FSD As New Microsoft.Win32.OpenFileDialog
            FSD.Filter = "Supported Formats (*.xml,*.csv)|*.csv;*.xml"
            FSD.InitialDirectory = My.Settings.LAST_IMPORT_DIR
            FSD.Multiselect = False
            If FSD.ShowDialog Then
                If String.IsNullOrEmpty(FSD.FileName) Then
                    Return ""
                Else
                    My.Settings.LAST_IMPORT_DIR = Path.GetDirectoryName(FSD.FileName)
                    My.Settings.Save()
                    Return FSD.FileName
                End If
            Else
                Return ""
            End If
        Catch ex As Exception
            AddConsoleLine("Problem with SelectCSVFile(). Error: " & ex.Message)
            Return ""
        End Try
    End Function

    Private Function PresentProjectSetupDialog() As Boolean
        Try
            Dim dlg As New ProjectSetup_Dialog

            dlg.ProjectName = PROJECT.Name
            dlg.ProductId = PROJECT.Product_Id
            dlg.ProductLanguage = PROJECT.Product_Lang
            dlg.ProductTitle = PROJECT.Product_Title
            dlg.ProductReleaseDate = PROJECT.Product_ReleasedDate
            dlg.ProductStudio = PROJECT.Product_Studio
            dlg.ProductType = PROJECT.Product_Type
            dlg.DVDPath = PROJECT.DVDPath
            dlg.ParentWorkId = PROJECT.Product_ParentWorkId

            Dim b As Boolean = dlg.ShowDialog
            If b Then
                PROJECT.Name = dlg.ProjectName
                PROJECT.Product_Id = dlg.ProductId
                PROJECT.Product_Lang = dlg.ProductLanguage
                PROJECT.Product_Title = dlg.ProductTitle
                PROJECT.Product_ReleasedDate = dlg.ProductReleaseDate
                PROJECT.Product_Studio = dlg.ProductStudio
                PROJECT.Product_Type = dlg.ProductType
                PROJECT.DVDPath = dlg.DVDPath
                PROJECT.Product_ParentWorkId = dlg.ParentWorkId
                UpdateProjectPropertyDisplay()
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Me.AddConsoleLine("Problem with PresentProjectSetupDialog(). Error: " & ex.Message)
            Return False
        End Try
    End Function

    Private Sub ShowSceneEditor(ByRef SI As cRCDb_SCENE_ITEM)
        Dim dlg As New SceneEditor_Dialog(SI)
        dlg.Owner = Me
        Dim b As Boolean = dlg.ShowDialog
        CollectTab_UpdateProjectDisplay()
        VerifyTab_UpdateDisplay()
        PROJECT.EVENTS.ForceRefresh()
    End Sub

    Private Sub ShowCastItemEditor(ByRef CI As cRCDb_CONTRIBUTOR_ITEM)
        Dim dlg As New CastEditor_Dialog(CI)
        dlg.Owner = Me
        Dim b As Boolean = dlg.ShowDialog
        CollectTab_UpdateProjectDisplay()
        VerifyTab_UpdateDisplay()
        PROJECT.EVENTS.ForceRefresh()
    End Sub

    Private Sub ShowMusicEditor(ByRef MI As cRCDb_MUSIC_ITEM)
        Dim dlg As New TrackEditor_Dialog(MI)
        dlg.Owner = Me
        Dim b As Boolean = dlg.ShowDialog
        CollectTab_UpdateProjectDisplay()
        VerifyTab_UpdateDisplay()
        PROJECT.EVENTS.ForceRefresh()
    End Sub

    Private Sub ShowFactEditor(ByRef FI As cRCDb_FACT_ITEM)
        Dim dlg As New FactEditor_Dialog(FI)
        dlg.Owner = Me
        Dim b As Boolean = dlg.ShowDialog
        CollectTab_UpdateProjectDisplay()
        VerifyTab_UpdateDisplay()
        PROJECT.EVENTS.ForceRefresh()
    End Sub

#End Region 'FUNCTIONALITY:DIALOGS

#Region "FUNCTIONALITY:EXPORT"

    Private Sub ExportMovieIQ()
        Try
            Dim MIQPath As String

            'SET OUTPUT PATH
            Dim FSD As New Microsoft.Win32.SaveFileDialog
            FSD.Filter = "Movie IQ XML|*.xml"
            FSD.FileName = PROJECT.Name & "_MovieIQ.xml"
            FSD.InitialDirectory = My.Settings.LAST_EXPORT_DIR
            If FSD.ShowDialog Then
                If Not String.IsNullOrEmpty(FSD.FileName) Then
                    MIQPath = FSD.FileName
                    My.Settings.LAST_EXPORT_DIR = Path.GetDirectoryName(FSD.FileName)
                    My.Settings.Save()
                Else
                    Exit Sub
                End If
            Else
                Exit Sub
            End If

            'OUTPUT MIQ
            If PROJECT.ExportMovieIQ(MIQPath) Then
                MsgBox("Movie IQ output succeeded and validated.")
            Else
                MsgBox("Movie IQ output failed.")
            End If

        Catch ex As Exception
            AddConsoleLine("Problem with ExportMovieIQ(). Error: " & ex.Message)
        End Try
    End Sub

#End Region 'FUNCTIONALITY:EXPORT

#End Region 'FUNCTIONALITY

#Region "GUI"

    Private Sub btnScrap_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        SaveProjectWithDialog()
    End Sub

#Region "GUI:MENUS"

    Private Sub miNewProject_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles miNewProject.Click
        NewBlankProject()
    End Sub

    Private Sub miOpenProject_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles miOpenProject.Click
        SelectAndLoadProject()
    End Sub

    Private Sub miSaveProject_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles miSaveProject.Click
        SaveProjectCore()
    End Sub

    Private Sub miCloseProject_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles miCloseProject.Click
        NewBlankProject()
    End Sub

    Private Sub miSaveProjectAs_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles miSaveProjectAs.Click
        SaveProjectWithDialog()
    End Sub

    Private Sub miExportMovieIQ_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles miExportMovieIQ.Click
        ExportMovieIQ()
    End Sub

    Private Sub miExit_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles miExit.Click
        ExitRoutiene()
    End Sub

    Private Sub miNewEventSet_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        EventVizualizer.NewSet()
    End Sub

    Private Sub miDeleteEventSet_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles miDeleteEventSet.Click
        EventVizualizer.DeleteSet()
    End Sub

    Private Sub miSetJumpSeconds_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles miSetJumpSeconds.Click
        Dim s As String = InputBox("Jump Seconds", "Set Jump Seconds", My.Settings.JUMP_SECONDS)
        If Not IsNumeric(s) OrElse CUShort(s) > 32000 OrElse CUShort(s) < 0 Then
            MsgBox("Invalid input.")
            Exit Sub
        End If
        My.Settings.JUMP_SECONDS = s
        My.Settings.Save()
    End Sub

#End Region 'GUI:MENUS

#Region "GUI:DVD PB"

    Private Sub btnRM_root_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnRM_root.Click
        If PLAYER Is Nothing Then Exit Sub
        PLAYER.GoToMenu(DvdMenuID.Root)
    End Sub

    Private Sub btnRM_title_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnRM_title.Click
        If PLAYER Is Nothing Then Exit Sub
        PLAYER.GoToMenu(DvdMenuID.Title)
    End Sub

    Private Sub btnRM_left_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnRM_left.Click
        If PLAYER Is Nothing Then Exit Sub
        PLAYER.DirectionalBtnHit(DvdRelButton.Left)
    End Sub

    Private Sub btnRM_right_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnRM_right.Click
        If PLAYER Is Nothing Then Exit Sub
        PLAYER.DirectionalBtnHit(DvdRelButton.Right)
    End Sub

    Private Sub btnRM_up_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnRM_up.Click
        If PLAYER Is Nothing Then Exit Sub
        PLAYER.DirectionalBtnHit(DvdRelButton.Upper)
    End Sub

    Private Sub btnRM_down_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnRM_down.Click
        If PLAYER Is Nothing Then Exit Sub
        PLAYER.DirectionalBtnHit(DvdRelButton.Lower)
    End Sub

    Private Sub btnRM_enter_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnRM_enter.Click
        If PLAYER Is Nothing Then Exit Sub
        PLAYER.EnterBtn()
    End Sub

    Private Sub btnRM_play_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnRM_play.Click
        If PLAYER Is Nothing Then
            InitializePlayer()
        Else
            PLAYER.Play()
        End If
    End Sub

    Private Sub btnRM_pause_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnRM_pause.Click
        If PLAYER Is Nothing Then Exit Sub
        PLAYER.Pause()
    End Sub

    Private Sub btnRM_stop_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnRM_stop.Click
        If PLAYER Is Nothing Then Exit Sub
        PLAYER.Stop()
    End Sub

    Private Sub btnRM_chback_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnRM_chback.Click
        If PLAYER Is Nothing Then Exit Sub
        PLAYER.PreviousChapter()
    End Sub

    Private Sub btnRM_rw_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnRM_rewind.Click
        If PLAYER Is Nothing Then Exit Sub
        PLAYER.Rewind()
    End Sub

    Private Sub btnRM_ff_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnRM_fastforward.Click
        If PLAYER Is Nothing Then Exit Sub
        PLAYER.FastForward()
    End Sub

    Private Sub btnRM_chnext_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnRM_chnext.Click
        If PLAYER Is Nothing Then Exit Sub
        PLAYER.NextChapter()
    End Sub

    Private Sub btnRM_audcycle_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnRM_audcycle.Click

    End Sub

    Private Sub btnRM_angcycle_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnRM_angcycle.Click

    End Sub

    Private Sub btnRM_subcycle_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnRM_subcycle.Click

    End Sub

    Private Sub btnRM_jumpback_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnRM_jumpback.Click
        If PLAYER Is Nothing Then Exit Sub
        PLAYER.JumpBack(My.Settings.JUMP_SECONDS)
    End Sub

    Private Sub btnRM_togglecaptions_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnRM_ToggleClosedCaptions.Click
        Try
            If PLAYER Is Nothing Then Exit Sub
            If Not PLAYER.ToggleClosedCaptions() Then Throw New Exception("Problem with UserToggleCCs in btnCCs_Click.")
        Catch ex As Exception
            AddConsoleLine("Problem changing CC display status. error: " & ex.Message)
        End Try
    End Sub

    Private Sub btnRM_Resync_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnRM_Resync.Click
        Try
            If PLAYER Is Nothing Then Exit Sub
            PLAYER.ReSyncAudio(False)
        Catch ex As Exception
            AddConsoleLine("Problem resyncing. error: " & ex.Message)
        End Try
    End Sub

#End Region 'GUI:DVD PB

#Region "GUI:TABS"

#Region "GUI:TABS:OVERVIEW"

#End Region 'GUI:TABS:OVERVIEW

#Region "GUI:TABS:PREPARE"

    Private Sub btn_PRE_CON_Import_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_PRE_CON_Import.Click
        PromptImportContributors()
    End Sub

    Private Sub btn_PRE_FO_Import_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_PRE_FO_Import.Click
        PromptImportFilmography()
    End Sub

    Private Sub btn_PRE_ST_Import_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_PRE_ST_Import.Click
        PromptImportSoundtrack()
    End Sub

    Private Sub btn_PRE_FT_Import_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_PRE_FT_Import.Click
        PromptImportFacts()
    End Sub

    Private Sub btn_PRE_FT_Clear_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_PRE_FT_Clear.Click
        ClearFacts()
    End Sub

    Private Sub btn_PRE_ST_Clear_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_PRE_ST_Clear.Click
        ClearSoundtrack()
    End Sub

    Private Sub btn_PRE_FO_Clear_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_PRE_FO_Clear.Click
        ClearFilmography()
    End Sub

    Private Sub btn_PRE_CON_Clear_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_PRE_CON_Clear.Click
        ClearContributors()
    End Sub

    Private Sub btnPRE_GN_Edit_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnPRE_GN_Edit.Click
        PresentProjectSetupDialog()
    End Sub

    Private Sub lblPRE_GN_DVDTitleNumber_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblPRE_GN_DVDTitleNumber.MouseLeftButtonDown
        Dim tt As String = InputBox("Which DVD Title Number do you wish to associate project with?", My.Settings.APPLICATION_NAME_SHORT, 1)
        If String.IsNullOrEmpty(tt) OrElse Not IsNumeric(tt) Then
            MessageBox.Show("Invalid DVD Title Number.", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Warning)
            Exit Sub
        Else
            PROJECT.EVENTS.DVDTitleNumber = tt
            Me.lblPRE_GN_DVDTitleNumber.Content = tt
        End If
    End Sub

    Private Sub lblPRE_GN_RunningTimeOffset_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblPRE_GN_RunningTimeOffset.MouseLeftButtonUp
        Dim tt As String = InputBox("Enter Offset In Seconds", My.Settings.APPLICATION_NAME_SHORT, PROJECT.EVENTS.RunningTimeOffset)
        If String.IsNullOrEmpty(tt) OrElse Not IsNumeric(tt) Then
            MessageBox.Show("Invalid offset.", My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Warning)
            Exit Sub
        Else
            PROJECT.EVENTS.RunningTimeOffset = tt
            Me.lblPRE_GN_RunningTimeOffset.Content = tt
        End If
    End Sub

    Private Sub btn_PRE_CON_Add_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_PRE_CON_Add.Click
        ShowContributorItemEditor(Nothing)
    End Sub

    Private Sub btn_PRE_CON_Delete_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_PRE_CON_Delete.Click
        PRE_CON_DeleteSelected()
    End Sub

    Private Sub btn_PRE_FLM_Delete_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_PRE_FLM_Delete.Click
        PRE_FLM_DeleteSelected()
    End Sub

    Private Sub btn_PRE_ST_Delete_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_PRE_ST_Delete.Click
        PRE_ST_DeleteSelected()
    End Sub

    Private Sub btn_PRE_FT_Delete_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_PRE_FT_Delete.Click
        PRE_FT_DeleteSelected()
    End Sub

    Private Sub lvPR_Contributors_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lvPR_Contributors.MouseDoubleClick
        If lvPR_Contributors.SelectedItem Is Nothing Then Exit Sub
        ShowContributorItemEditor(lvPR_Contributors.SelectedItem)
    End Sub

    Private Sub btn_PRE_FLM_Add_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_PRE_FLM_Add.Click
        ShowFilmographyItemEditor(Nothing)
    End Sub

    Private Sub lvPR_Filmography_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lvPR_Filmography.MouseDoubleClick
        If lvPR_Filmography.SelectedItem Is Nothing Then Exit Sub
        ShowFilmographyItemEditor(lvPR_Filmography.SelectedItem)
    End Sub

    Private Sub btn_PRE_ST_Add_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_PRE_ST_Add.Click
        ShowSoundtrackItemEditor(Nothing)
    End Sub

    Private Sub lvPR_Soundtrack_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lvPR_Soundtrack.MouseDoubleClick
        If lvPR_Soundtrack.SelectedItem Is Nothing Then Exit Sub
        ShowSoundtrackItemEditor(lvPR_Soundtrack.SelectedItem)
    End Sub

    Private Sub btn_PRE_FT_Add_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_PRE_FT_Add.Click
        ShowFactItemEditor(Nothing)
    End Sub

    Private Sub lvPR_Facts_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lvPR_Facts.MouseDoubleClick
        If lvPR_Facts.SelectedItem Is Nothing Then Exit Sub
        ShowFactItemEditor(lvPR_Facts.SelectedItem)
    End Sub

#End Region 'GUI:TABS:PREPARE

#Region "GUI:TABS:COLLECT"

    Private SelectedCollectLV As Byte

#Region "GUI:TABS:COLLECT:SCENES"

    Private Sub btnCO_SC_Prev_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        MsgBox("what should this do?")
    End Sub

    Private Sub btnCO_SC_New_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnCO_SC_New.Click
        NewScene()
    End Sub

    Private Sub btnCO_SC_Next_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnCO_SC_Next.Click
        MsgBox("what should this do?")
    End Sub

    Private Sub btnCO_SC_Insert_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnCO_SC_Insert.Click
        MsgBox("what should this do?")
    End Sub

    Private Sub btnCO_SC_Edit_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnCO_SC_Edit.Click
        If lvCO_SC_Scenes.SelectedItem Is Nothing Then Exit Sub
        ShowSceneEditor(lvCO_SC_Scenes.SelectedItem)
    End Sub

    Private Sub btnCO_SC_Merge_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnCO_SC_Merge.Click
        MsgBox("inop")
    End Sub

    Private Sub btnCO_SC_Delete_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnCO_SC_Delete.Click
        DeleteSelectedScene()
    End Sub

    Private Sub lvCO_SC_Scenes_GotFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles lvCO_SC_Scenes.GotFocus
        SelectedCollectLV = 0
    End Sub

    Private Sub lvCO_SC_Scenes_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lvCO_SC_Scenes.MouseDoubleClick
        If lvCO_SC_Scenes.SelectedItem Is Nothing Then Exit Sub
        ShowSceneEditor(lvCO_SC_Scenes.SelectedItem)
    End Sub

#End Region 'GUI:TABS:COLLECT:SCENES

#Region "GUI:TABS:COLLECT:CAST"

    Private Sub lvCO_CS_Cast_GotFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles lvCO_CS_Cast.GotFocus
        SelectedCollectLV = 1
    End Sub

    Private Sub lvCO_CS_Cast_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lvCO_CS_Cast.MouseDoubleClick
        If lvCO_CS_Cast.SelectedItem Is Nothing Then Exit Sub
        ShowCastItemEditor(lvCO_CS_Cast.SelectedItem)
    End Sub

#End Region 'GUI:TABS:COLLECT:CAST

#Region "GUI:TABS:COLLECT:SOUNDTRACK"

    Private Sub lvCO_ST_Soundtrack_GotFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles lvCO_ST_Soundtrack.GotFocus
        SelectedCollectLV = 2
    End Sub

    Private Sub lvCO_ST_Soundtrack_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lvCO_ST_Soundtrack.MouseDoubleClick
        If lvCO_ST_Soundtrack.SelectedItem Is Nothing Then Exit Sub
        ShowMusicEditor(lvCO_ST_Soundtrack.SelectedItem)
    End Sub

#End Region 'GUI:TABS:COLLECT:SOUNDTRACK

#Region "GUI:TABS:COLLECT:FACTS"

    Private Sub lvCO_FC_Facts_GotFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles lvCO_FC_Facts.GotFocus
        SelectedCollectLV = 3
    End Sub

    Private Sub lvCO_FC_Facts_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lvCO_FC_Facts.MouseDoubleClick
        If lvCO_FC_Facts.SelectedItem Is Nothing Then Exit Sub
        ShowFactEditor(lvCO_FC_Facts.SelectedItem)
    End Sub

    Private Sub lvCO_FC_Facts_SizeChanged(ByVal sender As Object, ByVal e As System.Windows.SizeChangedEventArgs) Handles lvCO_FC_Facts.SizeChanged
        Me.gvcCT_FC_Desc.Width = lvCO_FC_Facts.ActualWidth - gvcCT_FC_Id.Width - 28
    End Sub

#End Region 'GUI:TABS:COLLECT:FACTS"

#Region "GUI:TABS:COLLECT:ITEM DRAGGING"

    Private DragStartPoint As System.Windows.Point

    Private Sub lvCO_CS_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lvCO_CS_Cast.MouseLeftButtonDown, lvCO_FC_Facts.MouseLeftButtonDown, lvCO_SC_Scenes.MouseLeftButtonDown, lvCO_ST_Soundtrack.MouseLeftButtonDown
        DragStartPoint = e.GetPosition(CType(sender, ListView))
    End Sub

    Private Sub lvCO_CS_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles lvCO_CS_Cast.MouseMove
        CollectTab_StartDrag(sender, e, eBolinas_EventType.Cast)
    End Sub

    Private Sub lvCO_FC_Facts_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles lvCO_FC_Facts.MouseMove
        CollectTab_StartDrag(sender, e, eBolinas_EventType.Fact)
    End Sub

    Private Sub lvCO_SC_Scenes_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles lvCO_SC_Scenes.MouseMove
        CollectTab_StartDrag(sender, e, eBolinas_EventType.Scene)
    End Sub

    Private Sub lvCO_ST_Soundtrack_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles lvCO_ST_Soundtrack.MouseMove
        CollectTab_StartDrag(sender, e, eBolinas_EventType.Music)
    End Sub

    Private Sub CollectTab_StartDrag(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs, ByVal Type As eBolinas_EventType)
        Dim senderLV As ListView = TryCast(sender, ListView)
        Dim mousePos As Windows.Point = e.GetPosition(senderLV)
        Dim diff As Vector = DragStartPoint - mousePos
        If e.LeftButton = MouseButtonState.Pressed And Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance And Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance Then
            If senderLV.SelectedIndex = -1 Then Exit Sub
            Dim DragDataObject As New DataObject(Type.ToString.ToLower, senderLV.SelectedItem)
            DragDrop.DoDragDrop(senderLV, DragDataObject, DragDropEffects.Move)
        End If
    End Sub

#End Region 'GUI:TABS:COLLECT:ITEM DRAGGING

#End Region 'GUI:TABS:COLLECT

#Region "GUI:TABS:VERIFY"

    Private Sub lvVF_FC_ActiveFacts_SizeChanged(ByVal sender As Object, ByVal e As System.Windows.SizeChangedEventArgs) Handles lvVF_FC_Facts.SizeChanged
        Me.gvcVF_FC_Description.Width = lvVF_FC_Facts.ActualWidth - gvcVF_FC_Id.Width - 28
    End Sub

    Private Sub lblCO_VF_SC_edit_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblCO_VF_SC_edit.MouseLeftButtonUp
        If VerifyTab_CurrentSceneItem Is Nothing Then Exit Sub
        ShowSceneEditor(VerifyTab_CurrentSceneItem)
    End Sub

    Private Sub lblCO_VF_ST_edit_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblCO_VF_ST_edit.MouseLeftButtonUp
        If VerifyTab_CurrentMusicItem Is Nothing Then Exit Sub
        ShowMusicEditor(VerifyTab_CurrentMusicItem)
    End Sub

    Private Sub lvVF_CS_Cast_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lvVF_CS_Cast.MouseDoubleClick
        If lvVF_CS_Cast.SelectedItem Is Nothing Then Exit Sub
        ShowCastItemEditor(lvVF_CS_Cast.SelectedItem)
    End Sub

    Private Sub lvVF_FC_Facts_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lvVF_FC_Facts.MouseDoubleClick
        If lvVF_FC_Facts.SelectedItem Is Nothing Then Exit Sub
        ShowFactEditor(lvVF_FC_Facts.SelectedItem)
    End Sub

    Private Sub lvVF_CS_Cast_MouseRightButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lvVF_CS_Cast.MouseRightButtonUp
        If lvVF_CS_Cast.SelectedItem Is Nothing Then Exit Sub
        ShowVerifyTabItemContextMenu(sender, e.GetPosition(sender))
    End Sub

    Private Sub lvVF_FC_Facts_MouseRightButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lvVF_FC_Facts.MouseRightButtonUp
        If lvVF_FC_Facts.SelectedItem Is Nothing Then Exit Sub
        ShowVerifyTabItemContextMenu(sender, e.GetPosition(sender))
    End Sub

    Private Sub lblCO_VF_SC_remove_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblCO_VF_SC_remove.MouseLeftButtonUp
        If VerifyTab_CurrentSceneItem Is Nothing Then Exit Sub
        MsgBox("nop")
    End Sub

    Private Sub lblCO_VF_ST_remove_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblCO_VF_ST_remove.MouseLeftButtonUp
        If VerifyTab_CurrentMusicItem Is Nothing Then Exit Sub
        MsgBox("nop")
    End Sub

#End Region 'GUI:TABS:VERIFY

#End Region 'GUI:TABS

#End Region 'GUI

#Region "KEYBOARD"

    Private Sub Main_Form_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles Me.KeyDown

    End Sub

    Private Sub Main_Form_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles Me.KeyUp
        'S = New Scene
        'X = Scroll downward through sets
        'D = New Event
        'F = Trim()
        'Space = pause
        'J = jump back
        'N = Rewind
        'M = Fast Forward
        'H = Chapter Back
        'K = Chapter Forward
        'Enter = Add selected item to current event in highlighted set
        'L = Tab between Scene, Cast, Soundtrack, Fact boxes (focus always goes to the top item)
        'Up/Down = Scroll through items in the selected Scene, Cast, Soundtrack or Fact boxes

        Select Case e.Key
            'Case Key.Up
            '    If tcMain.SelectedIndex = 2 And tcCollectVerify.SelectedIndex = 0 Then
            '        Dim si As Integer
            '        Select Case SelectedCollectLV
            '            Case 0
            '                si = lvCO_SC_Scenes.SelectedIndex
            '                If si > 0 Then
            '                    lvCO_SC_Scenes.SelectedIndex = si - 1
            '                Else
            '                    lvCO_SC_Scenes.SelectedIndex = lvCO_SC_Scenes.Items.Count - 1
            '                End If
            '                lvCO_SC_Scenes.ScrollIntoView(lvCO_SC_Scenes.SelectedItem)
            '            Case 1
            '                si = lvCO_CS_Cast.SelectedIndex
            '                If si > 0 Then
            '                    lvCO_CS_Cast.SelectedIndex = si - 1
            '                Else
            '                    lvCO_CS_Cast.SelectedIndex = lvCO_CS_Cast.Items.Count - 1
            '                End If
            '                lvCO_CS_Cast.ScrollIntoView(lvCO_CS_Cast.SelectedItem)
            '            Case 2
            '                si = lvCO_ST_Soundtrack.SelectedIndex
            '                If si > 0 Then
            '                    lvCO_ST_Soundtrack.SelectedIndex = si - 1
            '                Else
            '                    lvCO_ST_Soundtrack.SelectedIndex = lvCO_ST_Soundtrack.Items.Count - 1
            '                End If
            '                lvCO_ST_Soundtrack.ScrollIntoView(lvCO_ST_Soundtrack.SelectedItem)
            '            Case 3
            '                si = lvCO_FC_Facts.SelectedIndex
            '                If si > 0 Then
            '                    lvCO_FC_Facts.SelectedIndex = si - 1
            '                Else
            '                    lvCO_FC_Facts.SelectedIndex = lvCO_FC_Facts.Items.Count - 1
            '                End If
            '                lvCO_FC_Facts.ScrollIntoView(lvCO_FC_Facts.SelectedItem)
            '        End Select
            '    End If

            'Case Key.Down
            '    If tcMain.SelectedIndex = 2 And tcCollectVerify.SelectedIndex = 0 Then
            '        Dim si As Integer
            '        Select Case SelectedCollectLV
            '            Case 0
            '                si = lvCO_SC_Scenes.SelectedIndex
            '                If si = lvCO_SC_Scenes.Items.Count - 1 Then
            '                    lvCO_SC_Scenes.SelectedIndex = 0
            '                Else
            '                    lvCO_SC_Scenes.SelectedIndex = si + 1
            '                End If
            '                lvCO_SC_Scenes.ScrollIntoView(lvCO_SC_Scenes.SelectedItem)
            '            Case 1
            '                si = lvCO_CS_Cast.SelectedIndex
            '                If si = lvCO_CS_Cast.Items.Count - 1 Then
            '                    lvCO_CS_Cast.SelectedIndex = 0
            '                Else
            '                    lvCO_CS_Cast.SelectedIndex = si + 1
            '                End If
            '                lvCO_CS_Cast.ScrollIntoView(lvCO_CS_Cast.SelectedItem)
            '            Case 2
            '                si = lvCO_ST_Soundtrack.SelectedIndex
            '                If si = lvCO_ST_Soundtrack.Items.Count - 1 Then
            '                    lvCO_ST_Soundtrack.SelectedIndex = 0
            '                Else
            '                    lvCO_ST_Soundtrack.SelectedIndex = si + 1
            '                End If
            '                lvCO_ST_Soundtrack.ScrollIntoView(lvCO_ST_Soundtrack.SelectedItem)
            '            Case 3
            '                si = lvCO_FC_Facts.SelectedIndex
            '                If si = lvCO_FC_Facts.Items.Count - 1 Then
            '                    lvCO_FC_Facts.SelectedIndex = 0
            '                Else
            '                    lvCO_FC_Facts.SelectedIndex = si + 1
            '                End If
            '                lvCO_FC_Facts.ScrollIntoView(lvCO_FC_Facts.SelectedItem)
            '        End Select
            '    End If

            Case Key.X
                If Keyboard.Modifiers = ModifierKeys.Control Then
                    ExitRoutiene()
                End If
                EventVizualizer.SelectNextSet()

            Case Key.Enter
                If tcMain.SelectedIndex = 2 And tcCollectVerify.SelectedIndex = 0 Then
                    Select Case SelectedCollectLV
                        Case 0
                            If lvCO_SC_Scenes.SelectedItems.Count = 1 Then
                                EventVizualizer.AddDataItem(New DataObject("scene", lvCO_SC_Scenes.SelectedItem))
                            End If
                        Case 1
                            If lvCO_CS_Cast.SelectedItems.Count = 1 Then
                                EventVizualizer.AddDataItem(New DataObject("cast", lvCO_CS_Cast.SelectedItem))
                            End If
                        Case 2
                            If lvCO_ST_Soundtrack.SelectedItems.Count = 1 Then
                                EventVizualizer.AddDataItem(New DataObject("music", lvCO_ST_Soundtrack.SelectedItem))
                            End If
                        Case 3
                            If lvCO_FC_Facts.SelectedItems.Count = 1 Then
                                EventVizualizer.AddDataItem(New DataObject("fact", lvCO_FC_Facts.SelectedItem))
                            End If
                    End Select
                End If

            Case Key.L
                If tcMain.SelectedIndex = 2 And tcCollectVerify.SelectedIndex = 0 Then
                    Select Case SelectedCollectLV
                        Case 0
                            lvCO_CS_Cast.Focus()
                            lvCO_CS_Cast.SelectedIndex = 0
                        Case 1
                            lvCO_ST_Soundtrack.Focus()
                            lvCO_ST_Soundtrack.SelectedIndex = 0
                        Case 2
                            lvCO_FC_Facts.Focus()
                            lvCO_FC_Facts.SelectedIndex = 0
                        Case 3
                            lvCO_SC_Scenes.Focus()
                            lvCO_SC_Scenes.SelectedIndex = 0
                    End Select
                End If

            Case Key.D
                EventVizualizer.NewEvent()

            Case Key.F
                EventVizualizer.TrimCurrentEvent()

            Case Key.Space
                If PLAYER IsNot Nothing Then
                    PLAYER.Pause()
                End If

            Case Key.J
                If PLAYER IsNot Nothing Then
                    PLAYER.JumpBack(My.Settings.JUMP_SECONDS)
                End If

            Case Key.M
                If PLAYER IsNot Nothing Then
                    PLAYER.FastForward()
                End If

            Case Key.H
                If PLAYER IsNot Nothing Then
                    PLAYER.PreviousChapter()
                End If

            Case Key.K
                If PLAYER IsNot Nothing Then
                    PLAYER.NextChapter()
                End If

            Case Key.O
                If Keyboard.Modifiers = ModifierKeys.Control Then
                    SelectAndLoadProject()
                End If

            Case Key.N
                If Keyboard.Modifiers = ModifierKeys.Control Then
                    NewBlankProject()
                Else
                    If Not PLAYER Is Nothing Then
                        PLAYER.Rewind()
                    End If
                End If

            Case Key.S
                If Keyboard.Modifiers = ModifierKeys.Control Then
                    SaveProjectCore()
                    Exit Sub
                End If
                If (Keyboard.Modifiers = ModifierKeys.Control) And (Keyboard.Modifiers = ModifierKeys.Alt) Then
                    SaveProjectWithDialog()
                    Exit Sub
                End If
                EventVizualizer.NewSet()

            Case Key.Q
                If Keyboard.Modifiers = ModifierKeys.Control Then
                    ExportMovieIQ()
                End If

            Case Key.C
                If Keyboard.Modifiers = ModifierKeys.Control Then
                    NewBlankProject()
                End If

        End Select
    End Sub

#End Region 'KEYBOARD

#Region "UTILITY"

    Public Sub AddConsoleLine(ByVal Msg As String)
        MessageBox.Show(Msg, My.Settings.APPLICATION_NAME_SHORT, MessageBoxButton.OK, MessageBoxImage.Error)
    End Sub

    Public Function ContainsFocus(ByRef lv As ListView) As Boolean
        'If lv.IsFocused Then Return True
        'For Each c As Control In lv.Items
        '    If c.IsFocused Then Return True
        'Next
    End Function

#End Region 'UTILITY

    Private Sub Handle_EventSelected(ByRef e As cRCDb_EVENT_ITEM, ByRef SetName As String) Handles EventVizualizer.evEventSelected
        txtRM_EventStart.Text = New cTimecode(CUInt(e.Start), eFramerate.NTSC).ToString_NoFrames
        txtRM_EventEnd.Text = New cTimecode(CUInt(e.End), eFramerate.NTSC).ToString_NoFrames
        txtRM_EventName.Text = SetName
    End Sub

    Private Sub spBig_GotFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles spBig.GotFocus
        lvCO_SC_Scenes.Focus()
    End Sub

    Private Sub btnScrap_Click_1(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnScrap.Click
    End Sub

End Class
