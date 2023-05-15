Partial Public Class FactItemEditor_Dialog

    Public Property Id() As String
        Get
            Return txtId.Text
        End Get
        Set(ByVal value As String)
            txtId.Text = value
        End Set
    End Property

    Public Property FactReference() As String
        Get
            Return txtFactReference.Text
        End Get
        Set(ByVal value As String)
            txtFactReference.Text = value
        End Set
    End Property

    Public Property Description() As String
        Get
            Return txtDescription.Text
        End Get
        Set(ByVal value As String)
            txtDescription.Text = value
        End Set
    End Property

    Public Property Generic() As Boolean
        Get
            Return If(cbGeneric.SelectedIndex = 0, False, True)
        End Get
        Set(ByVal value As Boolean)
            cbGeneric.SelectedIndex = If(value, 1, 0)
        End Set
    End Property

    Public Property FactRefType() As eRCDb_FactReferenceType
        Get
            Return cbFactRefType.SelectedIndex
        End Get
        Set(ByVal value As eRCDb_FactReferenceType)
            cbFactRefType.SelectedIndex = value
        End Set
    End Property

    Public Property FactRefId() As String
        Get
            Return txtFactRefId.Text
        End Get
        Set(ByVal value As String)
            txtFactRefId.Text = value
        End Set
    End Property

    Public Property Source1() As String
        Get
            Return txtSource1.Text
        End Get
        Set(ByVal value As String)
            txtSource1.Text = value
        End Set
    End Property

    Public Property Source2() As String
        Get
            Return txtSource2.Text
        End Get
        Set(ByVal value As String)
            txtSource2.Text = value
        End Set
    End Property

    Public Property Source3() As String
        Get
            Return txtSource3.Text
        End Get
        Set(ByVal value As String)
            txtSource3.Text = value
        End Set
    End Property

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSave.Click
        'VALIDATE
        If txtId.Text <> "" And Not IsNumeric(txtId.Text) Then
            MsgBox("Id is invalid.")
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
