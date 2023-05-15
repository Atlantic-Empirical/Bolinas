Public Class Clip

    Public Shared Function GetToBounds(ByVal depObj As DependencyObject) As Boolean
        Return CBool(depObj.GetValue(ToBoundsProperty))
    End Function

    Public Shared Sub SetToBounds(ByVal depObj As DependencyObject, ByVal clipToBounds As Boolean)
        depObj.SetValue(ToBoundsProperty, clipToBounds)
    End Sub

    Public Shared ReadOnly ToBoundsProperty As DependencyProperty = DependencyProperty.RegisterAttached("ToBounds", GetType(Boolean), GetType(Clip), New PropertyMetadata(False, AddressOf OnToBoundsPropertyChanged))

    Private Shared Sub OnToBoundsPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
        Dim fe As FrameworkElement = TryCast(d, FrameworkElement)
        If fe IsNot Nothing Then
            ClipToBounds(fe)

            ' whenever the element which this property is attached to is loaded 
            ' or re-sizes, we need to update its clipping geometry 
            AddHandler fe.Loaded, AddressOf fe_Loaded
            AddHandler fe.SizeChanged, AddressOf fe_SizeChanged
        End If
    End Sub

    ''' <summary> 
    ''' Creates a rectangular clipping geometry which matches the geometry of the 
    ''' passed element 
    ''' </summary> 
    Private Shared Sub ClipToBounds(ByVal fe As FrameworkElement)
        If GetToBounds(fe) Then
            Dim rg As New RectangleGeometry
            rg.Rect = New Rect(0, 0, fe.ActualWidth, fe.ActualHeight)
            fe.Clip = rg
        Else
            fe.Clip = Nothing
        End If
    End Sub

    Private Shared Sub fe_SizeChanged(ByVal sender As Object, ByVal e As SizeChangedEventArgs)
        ClipToBounds(TryCast(sender, FrameworkElement))
    End Sub

    Private Shared Sub fe_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ClipToBounds(TryCast(sender, FrameworkElement))
    End Sub

End Class
