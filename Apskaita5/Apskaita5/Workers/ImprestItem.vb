Namespace Workers

    <Serializable()> _
    Public Class ImprestItem
        Inherits BusinessBase(Of ImprestItem)
        Implements IGetErrorForListItem

#Region " Business Methods "

        Private _Guid As Guid = Guid.NewGuid
        Private _ID As Integer = 0
        Private _FinancialDataCanChange As Boolean = True
        Private _PersonID As Integer = 0
        Private _PersonName As String = ""
        Private _PersonCode As String = ""
        Private _PersonCodeSodra As String = ""
        Private _ContractSerial As String = ""
        Private _ContractNumber As Integer = 0
        Private _IsChecked As Boolean = False
        Private _PayOffSumTotal As Double = 0
        Private _PayedOutDate As Date = Date.MaxValue


        Public ReadOnly Property ID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ID
            End Get
        End Property

        Public ReadOnly Property FinancialDataCanChange() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _FinancialDataCanChange
            End Get
        End Property

        Public ReadOnly Property PersonID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _PersonID
            End Get
        End Property

        Public ReadOnly Property PersonName() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _PersonName.Trim
            End Get
        End Property

        Public ReadOnly Property PersonCode() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _PersonCode.Trim
            End Get
        End Property

        Public ReadOnly Property PersonCodeSodra() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _PersonCodeSodra.Trim
            End Get
        End Property

        Public ReadOnly Property ContractSerial() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ContractSerial.Trim
            End Get
        End Property

        Public ReadOnly Property ContractNumber() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ContractNumber
            End Get
        End Property

        Public Property IsChecked() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _IsChecked
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Boolean)
                CanWriteProperty(True)
                If Not _FinancialDataCanChange Then Exit Property
                If _IsChecked <> value Then
                    _IsChecked = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property PayOffSumTotal() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_PayOffSumTotal)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If Not _FinancialDataCanChange Then Exit Property
                If CRound(_PayOffSumTotal) <> CRound(value) Then
                    _PayOffSumTotal = CRound(value)
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public ReadOnly Property PayedOutDate() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                If _PayedOutDate <> Date.MaxValue Then Return _PayedOutDate.ToString("yyyy-MM-dd")
                Return ""
            End Get
        End Property



        Public Function GetErrorString() As String _
            Implements IGetErrorForListItem.GetErrorString
            If IsValid Then Return ""
            Return "Klaida (-os) eilutėje '" & _PersonName & "': " & _
                Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Error)
        End Function

        Public Function GetWarningString() As String _
            Implements IGetErrorForListItem.GetWarningString
            If BrokenRulesCollection.WarningCount < 1 Then Return ""
            Return "Eilutėje '" & _PersonName & "' gali būti klaida: " _
                & Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Warning)
        End Function


        Protected Overrides Function GetIdValue() As Object
            Return _Guid
        End Function

        Public Overrides Function ToString() As String
            Return _PersonName
        End Function

#End Region

#Region " Validation Rules "

        Protected Overrides Sub AddBusinessRules()
            ValidationRules.AddRule(AddressOf ImprestAmmountValidation, "PayOffSumTotal")

            ValidationRules.AddDependantProperty("IsChecked", "PayOffSumTotal", False)
        End Sub

        ''' <summary>
        ''' Rule ensuring imprest ammount is entered.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function ImprestAmmountValidation(ByVal target As Object, _
          ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As ImprestItem = DirectCast(target, ImprestItem)

            If ValObj._IsChecked AndAlso Not CRound(ValObj._PayOffSumTotal) > 0 Then
                e.Description = "Pasirinktam darbuotojui " & ValObj.PersonName & " pagal darbo sutartį " & _
                    ValObj.ContractSerial & ValObj.ContractNumber & " nenurodytas avanso dydis."
                e.Severity = Validation.RuleSeverity.Error
                Return False
            End If

            Return True

        End Function


#End Region

#Region " Authorization Rules "

        Protected Overrides Sub AddAuthorizationRules()

        End Sub

#End Region

#Region " Factory Methods "

        Friend Shared Function GetImprestItem(ByVal dr As DataRow, _
            ByVal nFinancialDataCanChange As Boolean) As ImprestItem
            Return New ImprestItem(dr, nFinancialDataCanChange)
        End Function


        Private Sub New()
            ' require use of factory methods
            MarkAsChild()
        End Sub


        Private Sub New(ByVal dr As DataRow, ByVal nFinancialDataCanChange As Boolean)
            MarkAsChild()
            Fetch(dr, nFinancialDataCanChange)
        End Sub

#End Region

#Region " Data Access "

        Private Sub Fetch(ByVal dr As DataRow, ByVal nFinancialDataCanChange As Boolean)

            _ID = CIntSafe(dr.Item(0), 0)

            _PersonID = CIntSafe(dr.Item(0), 0)
            _PersonName = CStrSafe(dr.Item(1)).Trim
            _PersonCode = CStrSafe(dr.Item(2)).Trim
            _PersonCodeSodra = CStrSafe(dr.Item(3)).Trim
            _ContractSerial = CStrSafe(dr.Item(4)).Trim
            _ContractNumber = CIntSafe(dr.Item(5), 0)

            _FinancialDataCanChange = nFinancialDataCanChange

            If dr.Table.Columns.Count > 6 Then
                _PayOffSumTotal = CDblSafe(dr.Item(6), 2, 0)
                _PayedOutDate = CDateSafe(dr.Item(7), Date.MaxValue)
                _ID = CIntSafe(dr.Item(8), 0)
                _IsChecked = True
                MarkOld()
            Else
                _PayOffSumTotal = 0
                _ID = -1
                _IsChecked = False
                MarkNew()
            End If

        End Sub

        Friend Sub Insert(ByVal parent As ImprestSheet)

            Dim myComm As New SQLCommand("InsertImprestItem")
            myComm.AddParam("?PD", _PersonID)
            myComm.AddParam("?DS", _ContractSerial.Trim)
            myComm.AddParam("?DN", _ContractNumber)
            myComm.AddParam("?AD", parent.ID)
            myComm.AddParam("?SM", CRound(_PayOffSumTotal))

            myComm.Execute()

            _ID = Convert.ToInt32(myComm.LastInsertID)

            MarkOld()

        End Sub

        Friend Sub Update(ByVal parent As ImprestSheet)

            If _PayedOutDate <> Date.MaxValue Then Exit Sub

            Dim myComm As New SQLCommand("UpdateImprestItem")
            myComm.AddParam("?NR", _ID)
            myComm.AddParam("?SM", CRound(_PayOffSumTotal))

            myComm.Execute()

            MarkOld()

        End Sub

        Friend Sub DeleteSelf()

            If _PayedOutDate <> Date.MaxValue Then Exit Sub

            Dim myComm As New SQLCommand("DeleteImprestItem")
            myComm.AddParam("?NR", _ID)

            myComm.Execute()

            MarkNew()

        End Sub

#End Region

    End Class

End Namespace