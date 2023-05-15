


'#Region "PLAYBACK:PLAYER:EVENT HANDLING"

'Private Sub HandlePlayerInitialized() Handles PLAYER.evPlayerInitialized

'    ''ToggleRemote(True)

'    'lblVolumes.Text = PLAYER.VolumeCount
'    'lblCurrentVol.Text = PLAYER.CurrentVolume
'    'lblDiscSide.Text = PLAYER.DiscSide.ToString
'    'lblPublisher.Text = PLAYER.CurrentDVD.VMGM.ProviderID
'    'lblVTSsTotal.Text = PLAYER.VTSCount

'    'txtProjectPath.Text = PLAYER.DVDDirectory

'    'Me.btnPS_Eject.Enabled = True
'    'Me.btnPS_Reboot.Enabled = True

'    ''disc text
'    'lblDVDText.Text = PLAYER.DVDText
'    'ToolTip.SetToolTip(lblDVDText, lblDVDText.Text)
'    'If Not lblDVDText.Text = "Null" And Not lblDVDText.Text = "" Then
'    '    'VW.Text = "Phoenix - " & Me.lblDVDText.Text
'    'Else
'    '    If InStr(PLAYER.DVDDirectory.ToLower, "d:\", CompareMethod.Text) Then
'    '        'VW.Text = " " & Me.GetVolumeLabel("D") & " disc in DVD Drive"
'    '    End If
'    'End If

'    'Me.lbParentalManagement.Items.Clear()
'    'Dim PMs() As cParentalManagement_US = PLAYER.CurrentDVD.VMGM.GlobalTitleParentalManagement.Titles
'    'For Each PM As cParentalManagement_US In PMs
'    '    Me.lbParentalManagement.Items.Add(PM.ToString)
'    'Next

'    ''Me.CurrentUserProfile.AppOptions.Apply()

'    '' ADDED 080323 - buil 0.0.8 to hide the 3px top and bottom for NTSC content that keystone adds
'    'If Me.PLAYER.CurrentVideoStandard = eVideoStandard.NTSC Then
'    '    'Dim trim As Integer = (((Me.pnViewer.Height) / 480) * 3)
'    '    'PLAYER.SetViewerPositionInParent(0, 0 - trim, Me.pnViewer.Width, Me.pnViewer.Height)
'    '    'Me.pnViewer.Height -= trim
'    'End If

'End Sub

'Public Sub HandleSystemJacketPictureDisplayed() Handles PLAYER.evSystemJacketPictureDisplayed
'    'Me.btnPS_Reboot.Enabled = False
'    'btnPS_Eject.Enabled = False
'    'Me.miFILE_Eject.Enabled = False
'    'Me.miFILE_Browse.Enabled = True
'    'ToggleRemote(False)
'    'Me.HideWaitAnimation()
'End Sub

'Private Sub Handle_LB_OK_SET(ByVal Value As Boolean) Handles PLAYER.evLetterboxAllowed_SET
'    'lblVidAtt_lbok.Text = Value.ToString
'End Sub

'Private Sub Handle_PS_OK_SET(ByVal Value As Boolean) Handles PLAYER.evPanscanAllowed_SET
'    'lblVidAtt_psok.Text = Value.ToString
'End Sub

'Private Sub Handle_TileRegionMask_SET(ByVal Value() As Boolean) Handles PLAYER.evRegionMask_SET
'    'Reg_1.BackColor = GetBoolColor(Value(0))
'    'Reg_2.BackColor = GetBoolColor(Value(1))
'    'Reg_3.BackColor = GetBoolColor(Value(2))
'    'Reg_4.BackColor = GetBoolColor(Value(3))
'    'Reg_5.BackColor = GetBoolColor(Value(4))
'    'Reg_6.BackColor = GetBoolColor(Value(5))
'    'Reg_7.BackColor = GetBoolColor(Value(6))
'    'Reg_8.BackColor = GetBoolColor(Value(7))
'End Sub

'Private Sub Handle_TitleCount_SET(ByVal Value As Short) Handles PLAYER.evGlobalTitleCount_SET
'    'lblEXDB_TitleCount.Text = Value
'End Sub

'Private Sub Handle_VTSCount_SET(ByVal Value As Short) Handles PLAYER.evVTSCount_SET
'    'lblVTSsTotal.Text = Value
'End Sub

''Private Sub HandleClearConsole() Handles PLAYER.evClearConsole
''    Me.lvConsole.Items.Clear()
''End Sub

''Private Sub HandleConsoleLine(ByVal Type As eConsoleItemType, ByVal Msg As String, ByVal ExtendedInfo As String, ByVal GOPTC As cTimecode) Handles PLAYER.evConsoleLine
''    Me.AddConsoleLine(Type, Msg, ExtendedInfo, GOPTC)
''End Sub

''Private Sub HandleCurrentBitrate(ByVal Bitrate As Integer) Handles PLAYER.evCurrentBitrateNotification
''    UpdateBitrate(Bitrate)
''End Sub

'Private Sub HandleFrameRateChange(ByVal NewRate As Double) Handles PLAYER.evFrameRateChange
'    'lblCurrentFrameRate.Text = NewRate
'End Sub

'Private Sub HandlePlaybackStarted() Handles PLAYER.evPlaybackStarted
'    UpdateDashboard()
'End Sub

'Private Sub HandlePlaybackStopped(ByVal Success As Boolean) Handles PLAYER.evPlaybackStopped
'    btnRM_stop.Background = System.Windows.Media.Brushes.Gainsboro
'    btnRM_pause.Background = System.Windows.Media.Brushes.Gainsboro
'    btnRM_fastforward.Background = System.Windows.Media.Brushes.Gainsboro
'    btnRM_rewind.Background = System.Windows.Media.Brushes.Gainsboro
'    btnRM_stop.Background = System.Windows.Media.Brushes.DarkGray
'    btnRM_play.Background = System.Windows.Media.Brushes.Gainsboro
'End Sub

''Private Sub HandleProjectEjected(ByVal FromDoubleStopState As Boolean) Handles PLAYER.evProjectEjected
''    Try
''        ClearDashboard(False, True)
''        Me.btnPS_Eject.Enabled = False
''        Me.btnPS_Reboot.Enabled = False
''    Catch ex As Exception
''        Me.AddConsoleLine("Problem with HandleProjectEjected(). Error: " & ex.Message)
''    End Try
''End Sub

'Private Sub HandlePlaybackPaused(ByVal Paused As Boolean) Handles PLAYER.evPlaybackPaused
'    If Paused Then
'        btnRM_pause.Background = System.Windows.Media.Brushes.Gainsboro
'        btnRM_fastforward.Background = System.Windows.Media.Brushes.Gainsboro
'        btnRM_rewind.Background = System.Windows.Media.Brushes.Gainsboro
'        btnRM_stop.Background = System.Windows.Media.Brushes.Gainsboro
'        btnRM_play.Background = System.Windows.Media.Brushes.DarkGray
'    Else
'        btnRM_pause.Background = System.Windows.Media.Brushes.DarkGray
'        btnRM_fastforward.Background = System.Windows.Media.Brushes.Gainsboro
'        btnRM_rewind.Background = System.Windows.Media.Brushes.Gainsboro
'        btnRM_stop.Background = System.Windows.Media.Brushes.Gainsboro
'        btnRM_play.Background = System.Windows.Media.Brushes.Gainsboro
'    End If
'    UpdateDashboard()
'End Sub

'Private Sub HandleFastForward(ByVal Rate As Double) Handles PLAYER.evFastForward
'    btnRM_pause.Background = System.Windows.Media.Brushes.Gainsboro
'    btnRM_fastforward.Background = System.Windows.Media.Brushes.DarkGray
'    btnRM_rewind.Background = System.Windows.Media.Brushes.Gainsboro
'    btnRM_stop.Background = System.Windows.Media.Brushes.Gainsboro
'    btnRM_play.Background = System.Windows.Media.Brushes.Gainsboro
'End Sub

'Private Sub HandleRewind(ByVal Rate As Double) Handles PLAYER.evRewind
'    btnRM_pause.Background = System.Windows.Media.Brushes.Gainsboro
'    btnRM_fastforward.Background = System.Windows.Media.Brushes.Gainsboro
'    btnRM_rewind.Background = System.Windows.Media.Brushes.DarkGray
'    btnRM_stop.Background = System.Windows.Media.Brushes.Gainsboro
'    btnRM_play.Background = System.Windows.Media.Brushes.Gainsboro
'End Sub

'Private Sub HandleAudioCycled() Handles PLAYER.evAudioCycled
'    'Try
'    '    For i As Short = 0 To dgAudioStreams.VisibleRowCount - 1
'    '        dgAudioStreams.UnSelect(i)
'    '    Next
'    '    dgAudioStreams.Select(PLAYER.CurrentAudioStreamNumber)
'    '    'If Player.CurrentAudioStreamNumber = Player.CurrentAudioStreamCount Then
'    '    '    dgAudioStreams.Select(0)
'    '    'Else
'    '    'End If
'    'Catch ex As Exception
'    '    'Debug.WriteLine("Could not select current audio stream in the data grid.")
'    'End Try
'End Sub

'Private Sub HandleAudioStreamSet(ByVal StreamNumber As Short) Handles PLAYER.evAudioStreamSet
'    'lblCurrentAudACMOD.Text = ""
'    'lblCurrentAud_Bitrate.Text = ""
'    'lblCurrentAudio_ext.Text = ""
'    'lblCurrentAudio_format.Text = ""
'    'lblCurrentAudio_lang.Text = ""
'    'lblCurrentAudio_sn.Text = ""

'    'Try
'    '    For i As Short = 0 To dgAudioStreams.VisibleRowCount - 1
'    '        dgAudioStreams.UnSelect(i)
'    '    Next
'    '    dgAudioStreams.Select(StreamNumber)
'    'Catch ex As Exception
'    '    'Debug.WriteLine("Could not select current audio stream in the data grid.")
'    'End Try
'End Sub

'Private Sub HandleAudioStreamChanged(ByVal CurrentStream As Byte, ByVal NumberOfStreams As Byte) Handles PLAYER.evAudioStreamChanged
'    'Try
'    '    If PLAYER.PlayState = ePlayState.SystemJP Then Exit Sub

'    '    If CurrentStream = Nothing Or NumberOfStreams = Nothing Then
'    '        PLAYER.GetAudioStreamStatus(NumberOfStreams, CurrentStream)
'    '    End If

'    '    Dim A As seqAudio = PLAYER.GetAudio(CurrentStream)
'    '    If PLAYER.PlayState = ePlayState.SystemJP Then
'    '        lblCurrentAudio_lang.Text = ""
'    '        lblCurrentAudio_sn.Text = ""
'    '        lblCurrentAudio_ext.Text = ""
'    '        lblCurrentAudio_format.Text = ""
'    '        lblCurrentAudACMOD.Text = ""
'    '        lblCurrentAud_Bitrate.Text = ""
'    '    Else
'    '        lblCurrentAudio_lang.Text = A.Language
'    '        lblCurrentAudio_sn.Text = CurrentStream
'    '        lblCurrentAudio_ext.Text = A.Extension
'    '        lblCurrentAudio_format.Text = A.Format.ToUpper

'    '        Dim i As Integer = PLAYER.GetAudioBitrate

'    '        If InStr(A.Format.ToLower, "ac3") Then

'    '            i = PLAYER.GetAudioAC3_ACMOD
'    '            lblCurrentAudACMOD.Text = GetAC3ChannelMappingFromAppModeData(i)

'    '            i = PLAYER.GetAudioSurroundEncoded

'    '            If i = 2 Then
'    '                lblCurrentAudACMOD.Text &= " -SUR"
'    '            End If
'    '        ElseIf InStr(A.Format.ToLower, "dts") Then
'    '            lblCurrentAudACMOD.Text = ""
'    '            lblCurrentAud_Bitrate.Text = (i / 1000) & "k"
'    '        Else
'    '            lblCurrentAudACMOD.Text = ""
'    '        End If
'    '    End If

'    '    A = Nothing
'    'Catch ex As Exception
'    '    If PLAYER.StartingDVD Then Exit Sub
'    '    If PLAYER.PlayState = ePlayState.SystemJP Then Exit Sub
'    '    If InStr(ex.Message, "80040290", CompareMethod.Text) Then Exit Sub
'    '    AddConsoleLine("Problem with HandleAudioStreamChanged(). Error: " & ex.Message)
'    'End Try
'End Sub

'Private Sub DVDPlaybackRateReturnedToOneX() Handles PLAYER.evDVDPlaybackRateReturnedToOneX
'    btnRM_pause.Background = System.Windows.Media.Brushes.Gainsboro
'    btnRM_fastforward.Background = System.Windows.Media.Brushes.Gainsboro
'    btnRM_rewind.Background = System.Windows.Media.Brushes.Gainsboro
'    btnRM_stop.Background = System.Windows.Media.Brushes.Gainsboro
'    btnRM_play.Background = System.Windows.Media.Brushes.DarkGray
'End Sub

'Private Sub HandleSubtitleStreamChanged(ByVal NewStreamNumber As Byte) Handles PLAYER.evSubtitleStreamChanged
'    'Try
'    '    If NewStreamNumber = Nothing Then
'    '        NewStreamNumber = PLAYER.CurrentSubtitleStreamNumber()
'    '    End If

'    '    'For i As Short = 0 To dgSubtitles.VisibleRowCount - 1
'    '    '    dgSubtitles.UnSelect(i)
'    '    'Next

'    '    'If PLAYER.SubtitlesAreOn Then
'    '    '    dgSubtitles.Select(NewStreamNumber)
'    '    'End If

'    '    Debug.WriteLine("SubtitleStreamChanged()")
'    '    Try
'    '        If PLAYER.CurrentDomain = DvdDomain.VideoManagerMenu Or PLAYER.CurrentDomain = DvdDomain.VideoTitleSetMenu Or PLAYER.PlayState = ePlayState.SystemJP Then
'    '            lblCurrentSP_onoff.Text = ""
'    '            lblCurrentSP_lang.Text = ""
'    '            lblCurrentSP_sn.Text = ""
'    '            lblCurrentSP_ext.Text = ""
'    '            Exit Sub
'    '        End If

'    '        Dim S As seqSub = PLAYER.GetSubtitle(PLAYER.CurrentSubtitleStreamNumber)
'    '        lblCurrentSP_lang.Text = S.Language
'    '        lblCurrentSP_sn.Text = PLAYER.CurrentSubtitleStreamNumber
'    '        lblCurrentSP_ext.Text = S.Extension

'    '        'For i As Short = 0 To dgSubtitles.VisibleRowCount - 1
'    '        '    dgSubtitles.UnSelect(i)
'    '        'Next

'    '        If PLAYER.SubtitlesAreOn Then
'    '            lblCurrentSP_onoff.Text = "On"
'    '            Me.btnRM_ToggleSubtitles.ButtonStyle = BorderStyles.Office2003
'    '            'dgSubtitles.Select(PLAYER.CurrentSubtitleStreamNumber)
'    '        Else
'    '            lblCurrentSP_onoff.Text = "Off"
'    '            Me.btnRM_ToggleSubtitles.ButtonStyle = BorderStyles.Default
'    '        End If

'    '        S = Nothing

'    '    Catch ex As Exception
'    '        If PLAYER.StartingDVD Then Exit Sub
'    '        If PLAYER.PlayState = ePlayState.SystemJP Then Exit Sub
'    '        If InStr(ex.Message, "80040290", CompareMethod.Text) Then Exit Sub
'    '        AddConsoleLine("Problem with SubtitleStreamChanged(). Error: " & ex.Message)
'    '    End Try
'    'Catch ex As Exception
'    '    'Debug.WriteLine("Could not select current subtitle stream in the data grid.")
'    'End Try
'End Sub

'Private Sub HandleAngleCycled(ByVal NewAngle As Byte) Handles PLAYER.evAngleCycled
'    'do anything? this is handled in angle changed below, no?
'End Sub

'Private Sub HandleClosedCaptionToggle() Handles PLAYER.evClosedCaptionToggle
'    If PLAYER.ClosedCaptionsAreOn Then
'        Me.btnRM_ToggleClosedCaptions.Background = System.Windows.Media.Brushes.Gainsboro
'    Else
'        Me.btnRM_ToggleClosedCaptions.Background = System.Windows.Media.Brushes.DarkGray
'    End If
'End Sub

'Private Sub HandleRunningTimeTick() Handles PLAYER.evRunningTimeTick
'    'UpdateDashboard()
'    'UpdatePlayPosition()

'    'If PLAYER.CurrentDomain = DvdDomain.VideoManagerMenu Or PLAYER.CurrentDomain = DvdDomain.VideoTitleSetMenu Then
'    '    If CompareDVDTimecodes(PLAYER.CurrentRunningTime, PLAYER.CurrentRunningTime) = 2 Then
'    '        'it has looped, store the loop time
'    '        lblRM_TotalRunningTime.Text = PLAYER.CurrentRunningTime.bHours.ToString & ":" & PadString(PLAYER.CurrentRunningTime.bMinutes.ToString, 2, "0", True) & ":" & PadString(PLAYER.CurrentRunningTime.bSeconds.ToString, 2, "0", True) '& ";" & F
'    '    End If
'    'End If
'End Sub

''Private Sub HandlePGCChange(ByVal NewPGC As Short) Handles PLAYER.evPGCChange
''    Try
''        If PLAYER.PlayState <> ePlayState.SystemJP And PLAYER.PlayState <> ePlayState.Init Then
''            Me.lblPGC.Text = NewPGC
''            Dim PGC As cPGC
''            If PLAYER.CurrentDomain = DvdDomain.Title Then

''                ''old way WORKS
''                'Dim loc As DvdPlayLocation
''                'Dim HR As Integer
''                'HR = Player.DVDInfo.GetCurrentLocation(loc)
''                'If HR < 0 Then Marshal.ThrowExceptionForHR(HR)
''                'Debug.WriteLine(loc.TitleNum & " - " & Player.CurrentTitle)
''                'PGC = Player.CurrentDVD.FindPGCByGTT(loc.TitleNum)

''                'New way
''                PGC = PLAYER.CurrentDVD.FindPGCByGTT(PLAYER.CurrentTitle)

''            ElseIf PLAYER.CurrentDomain = DvdDomain.VideoManagerMenu Or PLAYER.CurrentDomain = DvdDomain.VideoTitleSetMenu Then
''                PGC = PLAYER.CurrentDVD.VTSs(PLAYER.CurrentVTS - 1).VTSM_PGCI_UT.FindLUByLang("en").VXXM_PGCs(NewPGC - 1)
''            Else
''                lblCellsTotal.Text = ""
''                lblProgramsTotal.Text = ""
''                Exit Sub
''            End If

''            If Not PGC Is Nothing Then
''                lblCellsTotal.Text = PGC.CellCount
''                lblProgramsTotal.Text = PGC.ProgramCount
''            Else
''                lblPGC.Text = "x"
''                lblCellsTotal.Text = "x"
''                lblCell.Text = "x"
''                lblProgramsTotal.Text = "x"
''                lblProgramCur.Text = "x"
''            End If
''        Else
''            lblPGC.Text = ""
''            lblCellsTotal.Text = ""
''            lblProgramsTotal.Text = ""
''        End If
''    Catch ex As Exception
''        Debug.WriteLine("Problem with HandlePGCChange(). Error: " & ex.Message)
''    End Try
''End Sub

''Private Sub HandleKEYSTONE_INTERLACING(ByVal Interlaced As Boolean) Handles PLAYER.evKEYSTONE_Interlacing
''    If Not PLAYER.PlayState = ePlayState.SystemJP Then
''        'AddConsoleLine(eConsoleItemType.NOTICE, "Video source interlacing changed", Player.Interlaced, Nothing)
''        lblCurrentVideoIsInterlaced.Text = PLAYER.Interlaced.ToString
''    Else
''        lblCurrentVideoIsInterlaced.Text = ""
''    End If
''End Sub

''Public Sub HandleKEYSTONE_FIELDORDER(ByVal TopFieldFirst As Boolean) Handles PLAYER.evKEYSTONE_FieldOrder
''    If Not PLAYER.PlayState = ePlayState.SystemJP Then
''        Dim s As String
''        If TopFieldFirst Then
''            s = "Top"
''        Else
''            s = "Bottom"
''        End If

''        If s <> lbl32WhichField.Text Then
''            'AddConsoleLine("Video field order changed")
''            lbl32WhichField.Text = s
''        End If
''    Else
''        lbl32WhichField.Text = ""
''    End If
''End Sub

''Public Sub HandleMPEG_Timecode(ByVal TC As cTimecode) Handles PLAYER.evMPEG_Timecode
''    lblCurrentSourceTimecode.Text = TC.ToString_NoFrames
''End Sub

''Public Sub HandlePROGRESSIVE_SEQUENCE(ByVal ProgressiveSequence As Boolean) Handles PLAYER.evKEYSTONE_ProgressiveSequence

''End Sub

''Public Sub HandleMacrovisionStatus(ByVal Status As String) Handles PLAYER.evMacrovisionStatus
''    Me.lblMacrovision.Text = Status
''End Sub

''Public Sub HandleCurrentCellChanged(ByVal NewCell As Short) Handles PLAYER.evCellChange
''    Me.lblCell.Text = NewCell
''    ConfigBarDataBurnIn()
''End Sub

''Public Sub HandleCurrentPGChanged(ByVal NewPG As Short) Handles PLAYER.evProgramChange
''    Me.lblProgramCur.Text = NewPG
''    ConfigBarDataBurnIn()
''End Sub

''Public Sub HandleVTSChanged(ByVal NewVTS As Byte) Handles PLAYER.evVTSChange
''    Try
''        ConfigBarDataBurnIn()
''        If PLAYER.PlayState <> ePlayState.SystemJP And PLAYER.PlayState <> ePlayState.Init Then
''            Me.lblVTS.Text = NewVTS
''            'Me.CurrentVTS = p1
''            Dim VTS As cVTS = PLAYER.CurrentDVD.VTSs(NewVTS - 1)
''            lblVTSTTsTotal.Text = VTS.TitleCount
''            If PLAYER.CurrentDomain = DvdDomain.Title Then
''                lblPGCsTotal.Text = VTS.PGCCount_TitleSpace
''            ElseIf PLAYER.CurrentDomain = DvdDomain.VideoManagerMenu Or PLAYER.CurrentDomain = DvdDomain.VideoTitleSetMenu Then
''                lblPGCsTotal.Text = VTS.PGCCount_MenuSpace
''            Else
''                lblPGCsTotal.Text = "N/A"
''            End If

''            'Menu Language Sets
''            Dim Langs As List(Of String) = PLAYER.MenuLanguages_Strings
''            lbAvailableMenuLanguages.Items.Clear()
''            For Each s As String In Langs
''                lbAvailableMenuLanguages.Items.Add(s)
''            Next
''            'For ix As Short = UBound(Langs) To 0 Step -1
''            '    lbAvailableMenuLanguages.Items.Add(Langs(ix))
''            'Next
''            Me.HandlePGCChange(PLAYER.CurrentPGC)
''        Else
''            lblVTS.Text = ""
''            lblVTSTTsTotal.Text = ""
''            lblPGCsTotal.Text = ""
''        End If
''    Catch ex As Exception
''        Debug.WriteLine("Problem with HandleVTSChanged(). Error: " & ex.Message)
''    End Try
''End Sub

'Public Sub HandleChapterChanged(ByVal NewChapterNumber As Byte) Handles PLAYER.evChapterChange
'    'UpdateDashboard()
'    'If PLAYER.PlayState = ePlayState.SystemJP Then
'    '    lblCurrent_PTT.Text = ""
'    '    Me.lblDB_CurrentChapter.Text = ""
'    '    Me.lblDB_ChapterCount.Text = ""
'    '    lblPTTsTotal.Text = ""
'    'Else
'    '    lblCurrent_PTT.Text = NewChapterNumber
'    '    lblDB_CurrentChapter.Text = NewChapterNumber
'    '    lblPTTsTotal.Text = PLAYER.ChaptersInCurrentTitle
'    '    lblDB_ChapterCount.Text = PLAYER.ChaptersInCurrentTitle
'    'End If
'End Sub

''Public Sub HandleDomainChanged() Handles PLAYER.evDomainChange
''    Try
''        'Debug.WriteLine("Domain Change: " & PLAYER.CurrentDomain.ToString)
''        'AddConsoleLine("Domain Change")

''        EnableAllPositionLablesOnDB()
''        HandleVTSChanged(PLAYER.CurrentVTS) 'this is needed so that everything gets updated for a change between menu and title space in the same vts

''        gbRM_Misc.Enabled = True

''        If Not PLAYER.PlayState = ePlayState.SystemJP Then
''            Dim s As String
''            Select Case PLAYER.CurrentDomain
''                Case DvdDomain.FirstPlay
''                    s = "First Play"
''                    Me.ClearDashboard(True, False)

''                Case DvdDomain.Stop
''                    s = "Stop"
''                    Me.ClearDashboard(True, False)
''                    gbRM_Misc.Enabled = False
''                    btnRM_CallRootMenu.Enabled = True
''                    Me.btnRM_play.Enabled = True
''                    Me.btnRM_stop.Enabled = True

''                Case DvdDomain.Title
''                    s = "Title"
''                    HandleTitleChanged(-1)
''                    'TODO: IMPORTANT - need to change total PGCs to title space here
''                    Me.btnRM_SlowPlayback.Enabled = True

''                Case DvdDomain.VideoManagerMenu
''                    s = "VMGM"
''                    SetupSubtitleTab()
''                    DisablePositionLablesOnDBForMenuSpace()
''                    Me.btnRM_SlowPlayback.Enabled = False
''                    'TODO: IMPORTANT - need to change total PGCs to menu space here

''                Case DvdDomain.VideoTitleSetMenu
''                    s = "VTSM"
''                    SetupSubtitleTab()
''                    DisablePositionLablesOnDBForMenuSpace()
''                    Me.btnRM_SlowPlayback.Enabled = False
''                    'TODO: IMPORTANT - need to change total PGCs to menu space here

''            End Select
''            lblCurrentDomain.Text = s
''        End If

''        If PLAYER.CurrentDomain = DvdDomain.Title Then
''            Me.btnCopyAudioDescriptor.Enabled = True
''            Me.btnCopyVideoDescriptor.Enabled = True
''        Else
''            Me.btnCopyAudioDescriptor.Enabled = False
''            Me.btnCopyVideoDescriptor.Enabled = False
''        End If

''    Catch ex As Exception
''        AddConsoleLine("Problem with DomainChanged. Error: " & ex.Message)
''    End Try
''End Sub

'Public Sub HandleTitleChanged(ByVal NewTitle As Short) Handles PLAYER.evTitleChange
'    'Try
'    '    If PLAYER.CurrentDomain = DvdDomain.Stop Or PLAYER.CurrentDomain = DvdDomain.FirstPlay Or PLAYER.CurrentDomain = DvdDomain.VideoManagerMenu Or PLAYER.CurrentDomain = DvdDomain.VideoTitleSetMenu Then Exit Sub

'    '    If PLAYER.PlayState = ePlayState.SystemJP Then
'    '        Me.ClearDashboard(False, True)
'    '        Exit Sub
'    '    End If
'    '    Debug.WriteLine("Title Change. TTNo: " & NewTitle)

'    '    'This is needed for when a disc goes into a title for the first time
'    '    'the nav does not throw a titlechanged event when we've not yet been in any title
'    '    'so DomainChanged-Title calls TitleChange with "GetTitle"
'    '    If NewTitle = -1 Then

'    '        'Old way WORKS
'    '        'Dim L As DvdPlayLocation
'    '        'Player.DVDInfo.GetCurrentLocation(L)
'    '        'Debug.WriteLine(L.TitleNum & " - " & Player.CurrentTitle)
'    '        'NewTitle = L.TitleNum

'    '        'new way
'    '        NewTitle = PLAYER.CurrentTitle

'    '        If NewTitle = 0 Then
'    '            PopulateVideoStreamInfoWindow("TitleChange")
'    '            Me.lblTotalRunningTime.Text = "0:00:00"
'    '            Exit Sub
'    '        End If
'    '    End If

'    '    Me.lblDB_TitleCount.Text = PLAYER.GlobalTTCount
'    '    Me.lblDB_CurrentTitle.Text = NewTitle
'    '    Me.lblDB_CurrentChapter.Text = PLAYER.CurrentPlayLocation.ChapterNum
'    '    Me.lblDB_ChapterCount.Text = PLAYER.ChaptersInCurrentTitle

'    '    lblEXDB_CurrentTitle.Text = NewTitle
'    '    lblEXDB_TitleCount.Text = PLAYER.GlobalTTCount

'    '    lblCurrent_PTT.Text = PLAYER.CurrentPlayLocation.ChapterNum
'    '    lblPTTsTotal.Text = PLAYER.ChaptersInCurrentTitle

'    '    'Angles for TT
'    '    lbAvailAngles.Items.Clear()
'    '    lblAnglesTotal.Text = PLAYER.CurrentTitleAngleCount
'    '    lblCurrent_angle.Text = PLAYER.CurrentAngleStream
'    '    If PLAYER.CurrentTitleAngleCount > 0 Then
'    '        For i As Short = 1 To PLAYER.CurrentTitleAngleCount
'    '            lbAvailAngles.Items.Add(i)
'    '        Next
'    '    End If

'    '    'Timecode
'    '    CurrentTimecodeDisplayMode = "TTe"

'    '    Dim TC As DvdTimeCode
'    '    Dim M, S, F As String
'    '    If PLAYER.CurrentTitleTRT.bMinutes.ToString.Length = 1 Then
'    '        M = 0 & TC.bMinutes
'    '    Else
'    '        M = TC.bMinutes
'    '    End If

'    '    If PLAYER.CurrentTitleTRT.bSeconds.ToString.Length = 1 Then
'    '        S = 0 & TC.bSeconds
'    '    Else
'    '        S = TC.bSeconds
'    '    End If

'    '    Me.lblTotalRunningTime.Text = PLAYER.CurrentTitleTRT.bHours & ":" & M & ":" & S '& ";" & F

'    '    lblVTS.Text = PLAYER.CurrentVTS

'    '    lblVTSTT.Text = PLAYER.CurrentGlobalTT.VTS_TTN
'    '    Dim VTS As cVTS = PLAYER.CurrentDVD.VTSs(PLAYER.CurrentGlobalTT.VTSN - 1)
'    '    lblVTSTTsTotal.Text = VTS.TitleCount

'    '    If PLAYER.CurrentDomain = DvdDomain.Title Then
'    '        lblPGCsTotal.Text = VTS.PGCCount_TitleSpace
'    '    End If

'    '    Debug.WriteLine("SetupTitleMetaData()")
'    '    If Not PLAYER Is Nothing Then PLAYER.ClearOSD()
'    '    UpdateDashboard()

'    '    ''Subtitles
'    '    ''Dim MI As New MethodInvoker(AddressOf SSI.SetupSubtitleTab)
'    '    ''MI.BeginInvoke(Nothing, Nothing)

'    '    'SetupSubtitleTab()
'    '    Me.HandleSubtitleStreamChanged(PLAYER.CurrentSubtitleStreamNumber)

'    '    'Audio
'    '    SetupAudioTab()
'    '    Me.HandleAudioStreamChanged(Nothing, Nothing)

'    '    'Video
'    '    PopulateVideoStreamInfoWindow("TitleChange2")

'    'Catch ex As Exception
'    '    If PLAYER.StartingDVD Then Exit Sub
'    '    If PLAYER.PlayState = ePlayState.SystemJP Then Exit Sub
'    '    'If CheckEx(ex, "Title Change") Then Exit Sub
'    '    AddConsoleLine("Problem with HandleTitleChanged(). Error: " & ex.Message)
'    'End Try
'End Sub

''Public Sub HandleLayerbreakSet(ByVal Location As cNonSeamlessCell) Handles PLAYER.evLayerbreakSet
''    lblLBCh.Text = Location.PTT
''    lblLBTT.Text = Location.GTTn
''    lblLBTC.Text = Location.LBTCToString
''End Sub

'Public Sub HandleAngleChanged(ByVal NewAngle As Byte) Handles PLAYER.evAngleChanged
'    'PopulateVideoStreamInfoWindow("AngleChanged")
'    'lblCurrent_angle.Text = NewAngle
'    'ConfigBarDataBurnIn()
'    'If NewAngle <> 1 Then MsgBox("need to select the appropriate tab in the video descriptor tabs")
'End Sub

'Public Sub HandleJacketPicturesFound(ByVal JacketPicturePaths() As String) Handles PLAYER.evJacketPicturesFound
'    'lbJacketPictures.Items.Clear()
'    'For Each s As String In JacketPicturePaths
'    '    lbJacketPictures.Items.Add(New cJacketPicture(s))
'    'Next
'End Sub

''Private Sub HandlePlayerModeSet(ByVal Mode As ePlayerMode) Handles PLAYER.evPlayerModeSet
''    Me.AddConsoleLine(eConsoleItemType.NOTICE, "Player mode set to: " & Mode.ToString)
''End Sub

'Public Sub HandlePlayerKeyStrike(ByVal e As System.Windows.Forms.KeyEventArgs) Handles PLAYER.evKeyStrike
'    'Me.HandleKeyStrike(e)
'End Sub

'Public Sub HandleTripleStop() Handles PLAYER.evTripleStop
'    'Me.EjectProject()
'End Sub

'Private Sub HandlePlayerConsoleLine(ByVal Type As eConsoleItemType, ByVal Msg As String, ByVal ExtendedInfo As String, ByVal GOPTC As cTimecode) Handles PLAYER.evConsoleLine
'    'Debug.WriteLine(Msg)
'    'If Type = eConsoleItemType.WARNING Then
'    '    If InStr(Msg, "One of the DVD image files cannot be opened", CompareMethod.Text) Then
'    '        Me.AddConsoleLine("Problem reading one or more of the DVD files (IFOs & VOBs).")
'    '        Me.CloseProject()
'    '    End If
'    'End If
'End Sub

'Private Sub HandleWrongRegion(ByVal PlayerRegion As Byte, ByVal ProjectRegions() As Boolean) Handles PLAYER.evWrongRegion
'    Debug.WriteLine("guess it is still needed")
'    'PLAYER.PlayState = ePlayState.SystemJP 'to prevent the one second timer from trying to update the screen - graph is currently 'broken'
'    'Dim dlg As New WrongRegion_Dialog(PlayerRegion, ProjectRegions, Me)
'    'If dlg.ShowDialog() = DialogResult.OK Then
'    '    Me.BootDVD()
'    'Else
'    '    Me.CloseProject()
'    'End If
'End Sub

'#End Region 'PLAYBACK:PLAYER:EVENT HANDLING


'Public Sub EjectProject()
'    Try
'        If Not PLAYER.EjectProject() Then
'            'FATAL ERROR - APP MUST BE KILLED
'            If XtraMessageBox.Show(Me.LookAndFeel, Me, "Failed to eject project. Please restart " & My.Settings.APPLICATION_NAME & ".", My.Settings.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Stop) = MsgBoxResult.Ok Then
'                Application.Exit()
'            End If
'        Else
'            'ok, good
'        End If
'    Catch ex As Exception
'        Me.AddConsoleLine("Problem with EjectProject(). Error: " & ex.Message)
'    End Try
'End Sub




'Public Sub ResetRegions()
'    Reg_1.BackColor = GetBoolColor(True)
'    Reg_2.BackColor = GetBoolColor(True)
'    Reg_3.BackColor = GetBoolColor(True)
'    Reg_4.BackColor = GetBoolColor(True)
'    Reg_5.BackColor = GetBoolColor(True)
'    Reg_6.BackColor = GetBoolColor(True)
'    Reg_7.BackColor = GetBoolColor(True)
'    Reg_8.BackColor = GetBoolColor(True)
'End Sub




'#Region "PANELS:VIDEO STREAM INFO"

'#Region "PANELS:VIDEO STREAM INFO:POSITION SCROLLER"

'    Public Sub UpdatePlayPosition()
'        Try
'            If Not PLAYER.CurrentDomain = DvdDomain.Title OrElse PLAYER.PlayState = ePlayState.SystemJP Then
'                Me.tbPlayPosition.Value = 0
'                Exit Sub
'            End If
'            If Me.ScrollerMouseIsDown Then Exit Sub
'            Me.tbPlayPosition.Value = TimecodeToPercentage(PLAYER.CurrentRunningTime, PLAYER.TotalFrameCount, lblVidAtt_VidStd.Text)
'        Catch ex As Exception
'            Me.AddConsoleLine("Problem with UpdatePlayPosition. Error: " & ex.Message)
'        End Try
'    End Sub

'    Private Sub ExecuteScroll()
'        Try
'            If PLAYER Is Nothing Then Exit Sub
'            PLAYER.PlayAtTime(PercentageToTimecode(Me.tbPlayPosition.Value, PLAYER.TotalFrameCount, lblVidAtt_VidStd.Text), False)
'        Catch ex As Exception
'            Me.AddConsoleLine("Problem with ExecuteScroll(). Error: " & ex.Message)
'        End Try
'    End Sub

'    Private ScrollerMouseIsDown As Boolean = False
'    Private ScrollStartVal As Byte = 0
'    Private Sub tbPlayPosition_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles tbPlayPosition.MouseDown
'        Me.ScrollerMouseIsDown = True
'        Me.ScrollStartVal = Me.tbPlayPosition.Value

'        Me.ToolTip.AutomaticDelay = 0
'        Me.ToolTip.ShowAlways = True
'    End Sub

'    Private Sub tbPlayPosition_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles tbPlayPosition.MouseUp
'        Me.ScrollerMouseIsDown = False
'        If Me.ScrollStartVal <> Me.tbPlayPosition.Value Then
'            Me.ExecuteScroll()
'        End If

'        Me.ToolTip.AutomaticDelay = 500
'        Me.ToolTip.ShowAlways = False
'    End Sub

'    Private Sub tbPlayPosition_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tbPlayPosition.ValueChanged
'        If PLAYER Is Nothing Then Exit Sub
'        Dim TC As New cTimecode(PercentageToTimecode(Me.tbPlayPosition.Value, PLAYER.TotalFrameCount, lblVidAtt_VidStd.Text), (PLAYER.CurrentVideoStandard = eVideoStandard.NTSC))
'        Me.ToolTip.SetToolTip(sender, TC.ToString_NoFrames)
'    End Sub

'#End Region 'PANELS:VIDEO STREAM INFO:POSITION SCROLLER

'#Region "PANELS:VIDEO STREAM INFO *FUNCTIONALITY*"

'    Public Sub PopulateVideoStreamInfoWindow(ByVal Sender As String)
'        Try
'            'Debug.WriteLine("PopulateVideoTab()")
'            If PLAYER.PlayState = ePlayState.SystemJP Or PLAYER.CurrentDomain = DvdDomain.Stop Then
'                lblVidAtt_aspectratio.Text = ""
'                lblVidAtt_compression.Text = ""
'                lblVidAtt_frameheight.Text = ""
'                lblVidAtt_framerate.Text = ""
'                lblVidAtt_isFilmMode.Text = ""
'                lblVidAtt_VidStd.Text = ""
'                lblVidAtt_isSourceLetterboxed.Text = ""
'                lblVidAtt_lbok.Text = ""
'                lblVidAtt_line21Field1InGOP.Text = ""
'                lblVidAtt_line21Field2InGOP.Text = ""
'                lblVidAtt_psok.Text = ""
'                lblVidAtt_sourceResolution.Text = ""
'                'lblCurrentlyRunning32.Text = ""
'                lblCurrentFrameRate.Text = ""
'                lblCSSEncrypted.Text = ""
'                lblMacrovision.Text = ""
'                lbl32WhichField.Text = ""
'                lblCurrentVideoIsInterlaced.Text = ""
'                Exit Sub
'            End If


'            Dim VA As DvdVideoAttr = PLAYER.GetVideoAttributes

'            lblVidAtt_aspectratio.Text = VA.aspectX & " x " & VA.aspectY
'            lblVidAtt_compression.Text = VA.compression.ToString.ToUpper
'            lblVidAtt_frameheight.Text = VA.frameHeight
'            lblVidAtt_framerate.Text = VA.frameRate
'            lblVidAtt_isFilmMode.Text = VA.isFilmMode.ToString
'            lblVidAtt_isSourceLetterboxed.Text = VA.isSourceLetterboxed.ToString
'            lblVidAtt_line21Field1InGOP.Text = VA.line21Field1InGOP.ToString
'            Me.btnRM_ToggleClosedCaptions.Enabled = lblVidAtt_line21Field1InGOP.Text
'            lblVidAtt_line21Field2InGOP.Text = VA.line21Field2InGOP.ToString
'            lblVidAtt_sourceResolution.Text = VA.sourceResolutionX & " x " & VA.sourceResolutionY
'            Me.lblVidAtt_VidStd.Text = PLAYER.CurrentVideoStandard.ToString

'        Catch ex As Exception
'            AddConsoleLine("Problem populating video tab. error: " & ex.Message)
'        End Try
'    End Sub

'#End Region 'PANELS:VIDEO STREAM INFO *FUNCTIONALITY*

'#End Region 'PANELS:VIDEO STREAM INFO

'#Region "PANELS:AUDIO STREAM INFO"

'#Region "PANELS:AUDIO STREAM INFO *FUNCTIONALITY*"

'    Public Sub SetupAudioTab()
'        Try
'            'Debug.WriteLine("SetupAudioTab")
'            PLAYER.CloseAudDumpFile()
'            Dim NumberOfStreams, CurrentStream As Short
'            PLAYER.GetAudioStreamStatus(NumberOfStreams, CurrentStream)

'            Dim Audios As New seqAudioStreams
'            For i As Short = 0 To NumberOfStreams - 1
'                Audios.AddAudio(PLAYER.GetAudio(i))
'            Next
'            dgAudioStreams.DataSource = Audios.AudioStreams

'            'If NumberOfStreams < 1 Then
'            '    btnMute.Enabled = False
'            'Else
'            '    btnMute.Enabled = True
'            'End If

'            Try
'                For i As Short = 0 To dgAudioStreams.VisibleRowCount - 1
'                    dgAudioStreams.UnSelect(i)
'                Next
'                dgAudioStreams.Select(CurrentStream)
'            Catch ex As Exception
'                'Debug.WriteLine("Could not select current audio stream in the data grid.")
'            End Try


'            Dim gs As New DataGridTableStyle
'            gs.MappingName = Audios.AudioStreams.GetType.Name

'            Dim cs As New cDataGridNoActiveCellColumn
'            cs.MappingName = "StreamNumber"
'            cs.HeaderText = "ST"
'            cs.Alignment = HorizontalAlignment.Center
'            cs.Width = 40
'            gs.GridColumnStyles.Add(cs)

'            cs = New cDataGridNoActiveCellColumn
'            cs.MappingName = "Language"
'            cs.HeaderText = "Language"
'            cs.Alignment = HorizontalAlignment.Center
'            cs.Width = 60
'            gs.GridColumnStyles.Add(cs)

'            cs = New cDataGridNoActiveCellColumn
'            cs.MappingName = "Extension"
'            cs.HeaderText = "Extension"
'            cs.Alignment = HorizontalAlignment.Center
'            cs.Width = 75
'            gs.GridColumnStyles.Add(cs)

'            cs = New cDataGridNoActiveCellColumn
'            cs.MappingName = "Format"
'            cs.HeaderText = "Format"
'            cs.Alignment = HorizontalAlignment.Center
'            cs.Width = 60
'            gs.GridColumnStyles.Add(cs)

'            cs = New cDataGridNoActiveCellColumn
'            cs.MappingName = "NumberOfChannels"
'            cs.HeaderText = "CH"
'            cs.Alignment = HorizontalAlignment.Center
'            cs.Width = 40
'            gs.GridColumnStyles.Add(cs)

'            'cs = New cDataGridNoActiveCellColumn
'            'cs.MappingName = "AppMode"
'            'cs.HeaderText = "App Mode"
'            'cs.Alignment = HorizontalAlignment.Center
'            'cs.Width = 75
'            'gs.GridColumnStyles.Add(cs)

'            'cs = New cDataGridNoActiveCellColumn
'            'cs.MappingName = "Quantization"
'            'cs.HeaderText = "Quantization"
'            'cs.Alignment = HorizontalAlignment.Center
'            'cs.Width = 75
'            'gs.GridColumnStyles.Add(cs)

'            'cs = New cDataGridNoActiveCellColumn
'            'cs.MappingName = "Frequency"
'            'cs.HeaderText = "Frequency"
'            'cs.Alignment = HorizontalAlignment.Center
'            'cs.Width = 75
'            'gs.GridColumnStyles.Add(cs)

'            cs = New cDataGridNoActiveCellColumn
'            cs.MappingName = "Available"
'            cs.HeaderText = "Enabled"
'            cs.Alignment = HorizontalAlignment.Center
'            cs.Width = 60
'            gs.GridColumnStyles.Add(cs)

'            dgAudioStreams.TableStyles.Clear()
'            dgAudioStreams.TableStyles.Add(gs)

'        Catch ex As Exception
'            If InStr(ex.Message, "The data grid table styles collection already contains a table style with the same mapping name.", CompareMethod.Text) Then Exit Sub
'            AddConsoleLine("Problem with SetupAudioTab(). Error: " & ex.Message)
'        End Try
'    End Sub

'    Public Class cDataGridNoActiveCellColumn
'        Inherits DataGridTextBoxColumn
'        Private SelectedRow As Integer
'        'Fields
'        'Constructors
'        'Events
'        'Methods
'        Public Sub New()
'            'Warning: Implementation not found
'        End Sub
'        Protected Overloads Overrides Sub Edit(ByVal source As CurrencyManager, ByVal rowNum As Integer, ByVal bounds As Rectangle, ByVal [readOnly] As Boolean, ByVal instantText As String, ByVal cellIsVisible As Boolean)
'            'make sure selectedrow is valid
'            If (SelectedRow > -1) And (SelectedRow < source.List.Count + 1) Then
'                Me.DataGridTableStyle.DataGrid.UnSelect(SelectedRow)
'            End If
'            SelectedRow = rowNum
'            Me.DataGridTableStyle.DataGrid.Select(SelectedRow)

'        End Sub
'    End Class

'#End Region 'PANELS:AUDIO STREAM INFO *FUNCTIONALITY*

'#Region "PANELS:AUDIO STREAM INFO:EVENTS"

'    Private Sub dgAudioStreams_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) 'Handles dgAudioStreams.MouseUp
'        'Try
'        '    Dim HTI As System.Windows.Forms.DataGrid.HitTestInfo = dgAudioStreams.HitTest(e.X, e.Y)
'        '    If HTI.Row = -1 Then Exit Sub
'        '    dgAudioStreams.Select(HTI.Row)
'        '    Player.SetAudioStream(dgAudioStreams.CurrentRowIndex)
'        'Catch ex As Exception
'        '    Me.AddConsoleLine(eConsoleItemType.ERROR, "Problem with dgAudio stream click. Error: " & ex.Message)
'        'End Try
'    End Sub

'#End Region 'PANELS:AUDIO STREAM INFO:EVENTS

'#End Region 'PANELS:AUDIO STREAM INFO

'#Region "PANELS:SUBTITLE STREAM INFO"

'#Region "PANELS:SUBTITLE STREAM INFO *FUNCTIONALITY*"

'    Public Sub SetupSubtitleTab()
'        '        Try
'        '            If Player.PlayState = ePlayState.SystemJP Then Exit Sub
'        '            'Debug.WriteLine("SetupSubtitleTab()")

'        '            Dim NumberOfStreams As Integer = Player.CurrentDVD.GetSPCount(Player.CurrentVTS, (Player.CurrentDomain = DvdDomain.Title))
'        '            Dim Subtitles As seqSubs = New seqSubs
'        '            For i As Short = 0 To NumberOfStreams - 1
'        '                Subtitles.AddSub(Player.GetSubtitle(i))
'        '            Next

'        '            'ManualSubCBChange = True
'        '            'If SubIsDisabled Then
'        '            '    Me.cbDisplaySubtitles.Checked = False
'        '            'Else
'        '            '    Me.cbDisplaySubtitles.Checked = True
'        '            'End If
'        '            'ManualSubCBChange = False

'        '            ''do it in another thread
'        '            'Dim dlg As GetSubStreamStatusDelegate = New GetSubStreamStatusDelegate(AddressOf GetSubStreamStatus)
'        '            'dlg.BeginInvoke(AddressOf GetSubStreamStatusCompleted, Nothing)

'        '            Dim TempSubs(-1) As seqSub
'        '            For Each S As seqSub In Subtitles.SubtitleStreams
'        '                S.Enabled = Player.IsSubStreamEnabled(S.StreamNumber)
'        '                ReDim Preserve TempSubs(UBound(TempSubs) + 1)
'        '                TempSubs(UBound(TempSubs)) = S

'        '                'Try
'        '                '    HR = me.DVDCtrl.SelectSubpictureStream(S.StreamNumber, DvdCmdFlags.SendEvt, me.cmdOption)
'        '                '    If HR = -2147220849 Then
'        '                '        'Subtitle stream disabled in authoring
'        '                '        S.Enabled = False
'        '                '        'Marshal.ThrowExceptionForHR(HR)
'        '                '    ElseIf HR = -2147220874 Then
'        '                '        'Subtitle surfing disabled
'        '                '        S.Enabled = False
'        '                '        'Marshal.ThrowExceptionForHR(HR)
'        '                '    ElseIf HR < 0 Then
'        '                '        Marshal.ThrowExceptionForHR(HR)
'        '                '    Else
'        '                '        S.Enabled = True
'        '                '    End If
'        '                '    ReDim Preserve TempSubs(UBound(TempSubs) + 1)
'        '                '    TempSubs(UBound(TempSubs)) = S
'        '                'Catch ex As Exception
'        '                '    If InStr(ex.Message.ToLower, "8004028f", CompareMethod.Text) Or InStr(ex.Message.ToLower, "80040276", CompareMethod.Text) Then
'        '                '        'do nothing
'        '                '        GoTo NextSub
'        '                '    End If
'        '                '    Me.AddConsoleLine(eConsoleItemType.ERROR, "Problem checking availability of subs. Error: " & ex.Message)
'        '                'End Try
'        'NextSub:
'        '            Next

'        '            ''select stream 0
'        '            'If Not UBound(TempSubs) < 0 Then
'        '            '    Player.SetSubtitleStream(TempSubs(0).StreamNumber, False, False)
'        '            'End If

'        '            'Try
'        '            '    'return to the user selected stream number

'        '            '    If Player.SubtitlesAreOn Then
'        '            '        Player.SetSubtitleStream(Player.CurrentSubtitleStreamNumber, False, False)
'        '            '    Else
'        '            '        Player.SetSubtitleStream(Player.CurrentSubtitleStreamNumber, False, True)
'        '            '    End If
'        '            'Catch ex As Exception
'        '            '    'this code throws errors in sysjp so we'll just ignore them
'        '            'End Try









'        '            'Try
'        '            '    If DVDCtrl Is Nothing Or DVDInfo Is Nothing Then Exit Sub
'        '            '    Me.TurnSubtitlesOffOn(True)
'        '            'Catch ex As Exception
'        '            '    If InStr(ex.Message.ToLower, "8004028f", CompareMethod.Text) Then
'        '            '        TurnSubtitlesOffOn(False)
'        '            '        DVDCtrl.SelectSubpictureStream(0, DvdCmdFlags.SendEvt, cmdOption)
'        '            '        Me.AddConsoleLine("Subtitle stream " & StreamNumber & " is not available in the current title.")
'        '            '        Exit Sub
'        '            '    End If
'        '            '    If CheckEx(ex, "Set subtitles") Then Exit Sub
'        '            '    AddConsoleLine(eConsoleItemType.ERROR, "Problem toggling subtitle: " & ex.Message)
'        '            'End Try


'        '            dgSubtitles.DataSource = TempSubs

'        '            'If NumberOfStreams < 1 Then
'        '            '    me.btnToggleSubs.Enabled = False
'        '            'Else
'        '            '    me.btnToggleSubs.Enabled = True
'        '            'End If

'        '            Try
'        '                For i As Short = 0 To Me.dgSubtitles.VisibleRowCount - 1
'        '                    Me.dgSubtitles.UnSelect(i)
'        '                Next
'        '                If Player.SubtitlesAreOn Then
'        '                    Me.dgSubtitles.Select(Player.CurrentSubtitleStreamNumber)
'        '                End If
'        '            Catch ex As Exception
'        '                'Debug.WriteLine("Could not select current subtitle stream in the data grid.")
'        '            End Try

'        '            Dim gs As New DataGridTableStyle
'        '            gs.MappingName = Subtitles.SubtitleStreams.GetType.Name

'        '            Dim cs As New cDataGridNoActiveCellColumn
'        '            'Dim cs As New DataGridTextBoxColumn
'        '            cs.MappingName = "StreamNumber"
'        '            cs.HeaderText = "Stream"
'        '            cs.Alignment = HorizontalAlignment.Center
'        '            cs.Width = 50
'        '            gs.GridColumnStyles.Add(cs)

'        '            cs = New cDataGridNoActiveCellColumn
'        '            cs.MappingName = "Language"
'        '            cs.HeaderText = "Language"
'        '            cs.Alignment = HorizontalAlignment.Center
'        '            cs.Width = 100
'        '            gs.GridColumnStyles.Add(cs)

'        '            cs = New cDataGridNoActiveCellColumn
'        '            cs.MappingName = "Extension"
'        '            cs.HeaderText = "Extension"
'        '            cs.Alignment = HorizontalAlignment.Center
'        '            cs.Width = 200
'        '            gs.GridColumnStyles.Add(cs)

'        '            cs = New cDataGridNoActiveCellColumn
'        '            cs.MappingName = "Enabled"
'        '            cs.HeaderText = "Enabled"
'        '            cs.Alignment = HorizontalAlignment.Center
'        '            cs.Width = 50
'        '            gs.GridColumnStyles.Add(cs)

'        '            'cs = New DataGridNoActiveCellColumn
'        '            'cs.MappingName = "Type"
'        '            'cs.HeaderText = "Type"
'        '            'cs.Alignment = HorizontalAlignment.Center
'        '            'cs.Width = 75
'        '            'gs.GridColumnStyles.Add(cs)

'        '            'cs = New DataGridNoActiveCellColumn
'        '            'cs.MappingName = "Coding"
'        '            'cs.HeaderText = "Coding"
'        '            'cs.Alignment = HorizontalAlignment.Center
'        '            'cs.Width = 75
'        '            'gs.GridColumnStyles.Add(cs)

'        '            Me.dgSubtitles.TableStyles.Clear()
'        '            dgSubtitles.TableStyles.Add(gs)

'        '        Catch ex As Exception
'        '            If InStr(ex.Message, "The data grid table styles collection already contains a table style with the same mapping name.", CompareMethod.Text) Then Exit Sub
'        '            Me.AddConsoleLine(eConsoleItemType.ERROR, "Problem with SetupSubtitleTab(). Error: " & ex.Message)
'        '        End Try
'    End Sub

'#End Region 'PANELS:SUBTITLE STREAM INFO *FUNCTIONALITY*

'#Region "PANELS:SUBTITLE STREAM INFO:EVENTS"

'    Private Sub dgSubtitles_CurrentCellChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) 'Handles dgSubtitles.CurrentCellChanged
'        'Player.SetSubtitleStream(dgSubtitles.CurrentRowIndex, False, False)
'    End Sub

'#End Region 'PANELS:SUBTITLE STREAM INFO:EVENTS

'#End Region 'PANELS:SUBTITLE STREAM INFO

