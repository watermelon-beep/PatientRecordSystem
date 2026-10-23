Friend Class DrSettings
    Public Sub New()
    End Sub

    Public Shared Widening Operator CType(v As DrSettings) As DrSettings
        Throw New NotImplementedException()
    End Operator
End Class
