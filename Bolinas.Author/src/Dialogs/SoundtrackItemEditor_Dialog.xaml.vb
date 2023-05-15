Partial Public Class SoundtrackItemEditor_Dialog

    Public Property Id() As String
        Get
            Return txtId.Text
        End Get
        Set(ByVal value As String)
            txtId.Text = value
        End Set
    End Property

    Public Property Album() As String
        Get
            Return txtAlbum.Text
        End Get
        Set(ByVal value As String)
            txtAlbum.Text = value
        End Set
    End Property

    Public Property Artist() As String
        Get
            Return txtArtist.Text
        End Get
        Set(ByVal value As String)
            txtArtist.Text = value
        End Set
    End Property

    Public Property Time() As String
        Get
            Return txtTime.Text
        End Get
        Set(ByVal value As String)
            txtTime.Text = value
        End Set
    End Property

    Public Property Label() As String
        Get
            Return txtLabel.Text
        End Get
        Set(ByVal value As String)
            txtLabel.Text = value
        End Set
    End Property

    Public Property ReleaseDate() As Short
        Get
            Return txtReleaseDate.Text
        End Get
        Set(ByVal value As Short)
            txtReleaseDate.Text = value
        End Set
    End Property

    Public Property ReferenceId() As String
        Get
            Return txtReferenceId.Text
        End Get
        Set(ByVal value As String)
            txtReferenceId.Text = value
        End Set
    End Property

    Public Overloads Property Title() As String
        Get
            Return txtTitle.Text
        End Get
        Set(ByVal value As String)
            txtTitle.Text = value
        End Set
    End Property

    Public Property WorkId() As String
        Get
            Return txtWorkId.Text
        End Get
        Set(ByVal value As String)
            txtWorkId.Text = value
        End Set
    End Property

    Public Property Notes() As String
        Get
            Return txtNotes.Text
        End Get
        Set(ByVal value As String)
            txtNotes.Text = value
        End Set
    End Property

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSave.Click
        'VALIDATE
        If (txtId.Text <> "" And Not IsNumeric(txtId.Text)) OrElse txtId.Text > Integer.MaxValue Then
            MsgBox("Id is invalid.")
            Exit Sub
        End If
        If (txtReferenceId.Text <> "" And Not IsNumeric(txtReferenceId.Text)) OrElse txtReferenceId.Text > Integer.MaxValue Then
            MsgBox("ReferenceId is invalid.")
            Exit Sub
        End If
        If (txtWorkId.Text <> "" And Not IsNumeric(txtWorkId.Text)) OrElse txtWorkId.Text > Integer.MaxValue Then
            MsgBox("WorkId is invalid.")
            Exit Sub
        End If
        If (txtTime.Text <> "" And Not IsNumeric(txtTime.Text)) OrElse txtTime.Text > Integer.MaxValue Then
            MsgBox("Time is invalid.")
            Exit Sub
        End If
        'VALIDATE
        DialogResult = True
        Close()
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnCancel.Click
        DialogResult = False
        Close()
    End Sub

End Class
