Namespace General

    <Serializable()> _
    Public Class PersonGroup
        Inherits BusinessBase(Of PersonGroup)
        Implements IGetErrorForListItem

#Region " Business Methods "

        Private _Guid As Guid = Guid.NewGuid
        Private _ID As Integer = 0
        Private _Name As String = ""
        Private _OldName As String = ""
        Private _IsInUse As Boolean = False


        Public ReadOnly Property ID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ID
            End Get
        End Property

        Public Property Name() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Name.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _Name.Trim <> value.Trim Then
                    _Name = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public ReadOnly Property OldName() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _OldName.Trim
            End Get
        End Property

        Public ReadOnly Property IsInUse() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _IsInUse
            End Get
        End Property



        Public Function GetErrorString() As String _
            Implements IGetErrorForListItem.GetErrorString
            If IsValid Then Return ""
            Return "Klaida (-os) eilutėje '" & _Name & "': " _
                & Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Error)
        End Function

        Public Function GetWarningString() As String _
            Implements IGetErrorForListItem.GetWarningString
            If BrokenRulesCollection.WarningCount < 1 Then Return ""
            Return "Eilutėje '" & _Name & "' gali būti klaida: " _
                & Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Warning)
        End Function


        Protected Overrides Function GetIdValue() As Object
            Return _Guid
        End Function

        Public Overrides Function ToString() As String
            Return _Name
        End Function

#End Region

#Region " Validation Rules "

        Protected Overrides Sub AddBusinessRules()

            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("Name", "grupės pavadinimas"))
            ValidationRules.AddRule(AddressOf CommonValidation.ValueUniqueInCollection, _
                New CommonValidation.SimpleRuleArgs("Name", "grupės pavadinimas"))

        End Sub

#End Region

#Region " Authorization Rules "

        Protected Overrides Sub AddAuthorizationRules()

        End Sub

#End Region

#Region " Factory Methods "

        Friend Shared Function NewPersonGroup() As PersonGroup
            Dim result As New PersonGroup
            result.ValidationRules.CheckRules()
            Return result
        End Function

        Friend Shared Function GetPersonGroup(ByVal dr As DataRow) As PersonGroup
            Return New PersonGroup(dr)
        End Function



        Private Sub New()
            ' require use of factory methods
            MarkAsChild()
        End Sub

        Private Sub New(ByVal dr As DataRow)
            MarkAsChild()
            Fetch(dr)
        End Sub

#End Region

#Region " Data Access "

        Private Sub Fetch(ByVal dr As DataRow)

            _ID = CIntSafe(dr.item(0), 0)
            _Name = CStrSafe(dr.item(1)).Trim
            _OldName = _Name
            _IsInUse = ConvertDbBoolean(CIntSafe(dr.Item(2), 0))

            MarkOld()

        End Sub

        Friend Sub Insert(ByVal parent As PersonGroupList)

            Dim myComm As New SQLCommand("InsertPersonsGroup")
            myComm.AddParam("?GN", _Name.Trim)

            myComm.Execute()

            _ID = Convert.ToInt32(myComm.LastInsertID)

            MarkOld()

        End Sub

        Friend Sub Update(ByVal parent As PersonGroupList)

            Dim myComm As New SQLCommand("UpdatePersonsGroup")
            myComm.AddParam("?GN", _Name)
            myComm.AddParam("?GD", _ID)

            myComm.Execute()

            MarkOld()

        End Sub

        Friend Sub DeleteSelf()

            Dim myComm As New SQLCommand("DeletePersonsGroup")
            myComm.AddParam("?GD", _ID)
            myComm.Execute()

            MarkNew()

        End Sub


        Friend Sub CheckIfCanDelete()

            Dim myComm As New SQLCommand("GetPersonsBelongToGroup")
            myComm.AddParam("?GD", _ID)

            Using myData As DataTable = myComm.Fetch

                If myData.Rows.Count > 0 AndAlso CIntSafe(myData.Rows(0).Item(0), 0) > 0 Then _
                    Throw New Exception("Klaida. Grupei '" & _Name & "' yra priskirtų asmenų. " _
                    & "Jos pašalinti negalima.")

            End Using

        End Sub

#End Region

    End Class

End Namespace