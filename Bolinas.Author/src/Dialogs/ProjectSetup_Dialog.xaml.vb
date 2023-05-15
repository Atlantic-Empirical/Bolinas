Imports System.IO

Partial Public Class ProjectSetup_Dialog

    Public Property ProjectName() As String
        Get
            Return txtProjectName.Text
        End Get
        Set(ByVal value As String)
            txtProjectName.Text = value
        End Set
    End Property

    Public Property ProductId() As String
        Get
            Return txtProductId.Text
        End Get
        Set(ByVal value As String)
            txtProductId.Text = value
        End Set
    End Property

    Public Property ProductTitle() As String
        Get
            Return txtTitle.Text
        End Get
        Set(ByVal value As String)
            txtTitle.Text = value
        End Set
    End Property

    Public Property ProductReleaseDate() As String
        Get
            Return CType(dpReleaseDate.SelectedDate, Date).ToString("yyyy-MM-dd")
        End Get
        Set(ByVal value As String)
            Try
                dpReleaseDate.SelectedDate = [Date].Parse(value)
            Catch ex As Exception
                dpReleaseDate.SelectedDate = Today
            End Try
        End Set
    End Property

    Public Property ProductStudio() As String
        Get
            Return txtStudio.Text
        End Get
        Set(ByVal value As String)
            txtStudio.Text = value
        End Set
    End Property

    Public Property ProductType() As eBolinas_ProductType
        Get
            Return cbProductType.SelectedIndex
        End Get
        Set(ByVal value As eBolinas_ProductType)
            cbProductType.SelectedIndex = value
        End Set
    End Property

    Public Property ProductLanguage() As eBolinas_Language
        Get
            Return cbProductLang.SelectedIndex
        End Get
        Set(ByVal value As eBolinas_Language)
            cbProductLang.SelectedIndex = value
        End Set
    End Property

    Public Property DVDPath() As String
        Get
            Return txtDVDPath.Text
        End Get
        Set(ByVal value As String)
            txtDVDPath.Text = value
        End Set
    End Property

    Public Property ParentWorkId() As String
        Get
            Return txtParentWorkId.Text
        End Get
        Set(ByVal value As String)
            txtParentWorkId.Text = value
        End Set
    End Property

    Private Sub btnDone_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnDone.Click
        If Not Validate() Then
            MsgBox("Please complete all fields with valid data.")
            Exit Sub
        End If
        DialogResult = True
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnCancel.Click
        DialogResult = False
        Me.Close()
    End Sub

    Private Function Validate() As Boolean
        If txtDVDPath.Text = "" Then Return False
        If Not File.Exists(txtDVDPath.Text) Then Return False
        If txtProductId.Text = "" Then Return False
        If Not IsNumeric(txtProductId.Text) Then Return False
        If txtProjectName.Text = "" Then Return False
        If dpReleaseDate.SelectedDate.Value = Today Then Return False
        If txtStudio.Text = "" Then Return False
        If txtTitle.Text = "" Then Return False
        If txtParentWorkId.Text = "" Then Return False
        If Not IsNumeric(txtParentWorkId.Text) Then Return False
        Return True
    End Function

    Private Sub btnDVDPathBrowse_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnDVDPathBrowse.Click
        Try
TryAgain:
            Dim FSD As New Microsoft.Win32.OpenFileDialog
            FSD.Filter = "DVD (video_ts.ifo)|video_ts.ifo"
            FSD.InitialDirectory = My.Settings.LAST_VIDEOTS_DIR
            FSD.Multiselect = False
            If FSD.ShowDialog Then
                If FSD.FileName = "" Then
                    GoTo NullDVD
                Else
                    txtDVDPath.Text = FSD.FileName
                    My.Settings.LAST_VIDEOTS_DIR = Path.GetDirectoryName(FSD.FileName)
                    My.Settings.Save()
                End If
            Else
NullDVD:
                If MsgBox("DVD not selected. Would you like to try again?", MsgBoxStyle.YesNo, "Select DVD Path") = MsgBoxResult.Yes Then
                    GoTo TryAgain
                Else
                    MessageBox.Show("CRITICAL FAILURE: a DVD path must be selected in order to use this software. Application shutting down.", "FAILED TO SELECT DVD PATH", MessageBoxButton.OK, MessageBoxImage.Stop)
                    Process.GetCurrentProcess.Kill()
                End If
            End If
        Catch ex As Exception
            If MsgBox("Problem with SelectDVD(). Error: " & ex.Message & vbNewLine & "Try again?", MsgBoxStyle.YesNo And MsgBoxStyle.Critical) = MsgBoxResult.Yes Then
                GoTo TryAgain
            End If
        End Try
    End Sub

    Private Sub ProjectSetup_Dialog_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Me.txtProjectName.Focus()
        Me.cbProductType.SelectedIndex = 1
    End Sub

End Class
