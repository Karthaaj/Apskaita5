Namespace Documents

    <Serializable()> _
    Public Class CashAccount
        Inherits BusinessBase(Of CashAccount)
        Implements IGetErrorForListItem

#Region " Business Methods "

        Private _Guid As Guid = Guid.NewGuid
        Private _ID As Integer = 0
        Private _Type As CashAccountType = CashAccountType.BankAccount
        Private _ManagingPerson As PersonInfo
        Private _Account As Long = 0
        Private _BankFeeCostsAccount As Long = 0
        Private _Name As String = ""
        Private _BankAccountNumber As String = ""
        Private _BankName As String = ""
        Private _BankCode As String = ""
        Private _IsLitasEsisCompliant As Boolean = False
        Private _CurrencyCode As String = GetCurrentCompany.BaseCurrency
        Private _EnforceUniqueOperationID As Boolean = False
        Private _BankFeeLimit As Integer = 0
        Private _BalanceAtBegining As Double = 0
        Private _IsInUse As Boolean = False
        Private _IsObsolete As Boolean = False


        Public ReadOnly Property ID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ID
            End Get
        End Property

        Public Property [Type]() As CashAccountType
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As CashAccountType)
                If _IsInUse Then Exit Property
                CanWriteProperty(True)
                If _Type <> value Then
                    _Type = value
                    PropertyHasChanged()
                    PropertyHasChanged("TypeHumanReadable")
                End If
            End Set
        End Property

        Public Property TypeHumanReadable() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return ConvertEnumHumanReadable(_Type)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                If _IsInUse Then Exit Property
                CanWriteProperty(True)
                If (value Is Nothing OrElse String.IsNullOrEmpty(value.Trim)) AndAlso _
                    _Type = CashAccountType.BankAccount Then Exit Property
                If value Is Nothing OrElse String.IsNullOrEmpty(value.Trim) Then
                    _Type = CashAccountType.BankAccount
                    PropertyHasChanged()
                    PropertyHasChanged("Type")
                ElseIf _Type <> ConvertEnumHumanReadable(Of CashAccountType)(value) Then
                    _Type = ConvertEnumHumanReadable(Of CashAccountType)(value)
                    PropertyHasChanged()
                    PropertyHasChanged("Type")
                End If
            End Set
        End Property

        Public Property Account() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Account
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Long)
                CanWriteProperty(True)
                If _Account <> value Then
                    _Account = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property BankFeeCostsAccount() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _BankFeeCostsAccount
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Long)
                CanWriteProperty(True)
                If _BankFeeCostsAccount <> value Then
                    _BankFeeCostsAccount = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property ManagingPerson() As PersonInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ManagingPerson
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As PersonInfo)
                CanWriteProperty(True)
                If Not (_ManagingPerson Is Nothing AndAlso value Is Nothing) _
                    AndAlso Not (Not _ManagingPerson Is Nothing AndAlso Not value Is Nothing _
                    AndAlso _ManagingPerson = value) Then

                    _ManagingPerson = value
                    PropertyHasChanged()

                    If _Type = CashAccountType.BankAccount OrElse _Type = CashAccountType.PseudoBankAccount Then
                        If _ManagingPerson Is Nothing OrElse Not _ManagingPerson.ID > 0 Then
                            _BankCode = ""
                            _BankFeeCostsAccount = 0
                            _BankName = ""
                        Else
                            _BankCode = _ManagingPerson.Code
                            _BankFeeCostsAccount = _ManagingPerson.AccountAgainstBankSupplyer
                            _BankName = _ManagingPerson.Name
                        End If
                        PropertyHasChanged("BankCode")
                        PropertyHasChanged("BankFeeCostsAccount")
                        PropertyHasChanged("BankName")
                    End If

                End If
            End Set
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

        Public Property BankAccountNumber() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _BankAccountNumber.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _BankAccountNumber.Trim <> value.Trim Then
                    _BankAccountNumber = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property BankName() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _BankName.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _BankName.Trim <> value.Trim Then
                    _BankName = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property BankCode() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _BankCode.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _BankCode.Trim <> value.Trim Then
                    _BankCode = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property IsLitasEsisCompliant() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _IsLitasEsisCompliant
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Boolean)
                CanWriteProperty(True)
                If _IsLitasEsisCompliant <> value Then
                    _IsLitasEsisCompliant = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property CurrencyCode() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _CurrencyCode.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                If _IsInUse Then Exit Property
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _CurrencyCode.Trim <> value.Trim Then
                    _CurrencyCode = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property EnforceUniqueOperationID() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _EnforceUniqueOperationID
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Boolean)
                CanWriteProperty(True)
                If _EnforceUniqueOperationID <> value Then
                    _EnforceUniqueOperationID = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property BankFeeLimit() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _BankFeeLimit
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Integer)
                CanWriteProperty(True)
                If _BankFeeLimit <> value Then
                    _BankFeeLimit = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property BalanceAtBegining() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _BalanceAtBegining
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If value < 0 Then value = 0
                If CRound(_BalanceAtBegining) <> CRound(value) Then
                    _BalanceAtBegining = CRound(value)
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property IsObsolete() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _IsObsolete
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Boolean)
                CanWriteProperty(True)
                If _IsObsolete <> value Then
                    _IsObsolete = value
                    PropertyHasChanged()
                End If
            End Set
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
            Return "Klaida (-os) eilutėje '" & _Name & "': " & _
                Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Error)
        End Function

        Public Function GetWarningString() As String _
        Implements IGetErrorForListItem.GetWarningString
            If BrokenRulesCollection.WarningCount < 1 Then Return ""
            Return "Eilutėje '" & _Name & "' gali būti klaida: " & _
                Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Warning)
        End Function

        Protected Overrides Function GetIdValue() As Object
            Return _Guid
        End Function

        Public Overrides Function ToString() As String
            If Not _ID > 0 Then Return ""
            Return _Name
        End Function

#End Region

#Region " Validation Rules "

        Protected Overrides Sub AddBusinessRules()

            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("Name", "lėšų sąskaitos pavadinimas"))
            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("Account", "lėšų sąskaitos apskaitos sąskaita"))
            ValidationRules.AddRule(AddressOf CommonValidation.ValueUniqueInCollection, _
                New CommonValidation.SimpleRuleArgs("Name", "lėšų sąskaitos pavadinimas"))

            ValidationRules.AddRule(AddressOf ManagingPersonValidation, _
                New Validation.RuleArgs("ManagingPerson"))
            ValidationRules.AddRule(AddressOf BankFeeCostsAccountValidation, _
                New Validation.RuleArgs("BankFeeCostsAccount"))
            ValidationRules.AddRule(AddressOf BankValidation, New Validation.RuleArgs("BankAccountNumber"))
            ValidationRules.AddRule(AddressOf BankValidation, New Validation.RuleArgs("BankName"))
            ValidationRules.AddRule(AddressOf BankValidation, New Validation.RuleArgs("BankCode"))
            ValidationRules.AddRule(AddressOf CommonValidation.CurrencyValid, _
                New CommonValidation.SimpleRuleArgs("CurrencyCode", ""))

            ValidationRules.AddDependantProperty("Type", "ManagingPerson", False)
            ValidationRules.AddDependantProperty("Type", "BankFeeCostsAccount", False)
            ValidationRules.AddDependantProperty("Type", "BankAccountNumber", False)
            ValidationRules.AddDependantProperty("Type", "BankName", False)
            ValidationRules.AddDependantProperty("Type", "BankCode", False)

        End Sub

        ''' <summary>
        ''' Rule ensuring that the values of the properties 
        ''' related to Bank fees are valid (for (pseudo) bank account).
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function BankValidation(ByVal target As Object, _
        ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As CashAccount = DirectCast(target, CashAccount)

            If ValObj._Type <> CashAccountType.BankAccount AndAlso _
                ValObj._Type <> CashAccountType.PseudoBankAccount Then Return True

            Dim value As String = Convert.ToString(CallByName(target, e.PropertyName, CallType.Get))

            If value Is Nothing OrElse String.IsNullOrEmpty(value.Trim) Then
                e.Description = "Banko arba pseudo banko sąskaitai turi būti nurodyti " _
                    & "banko ar kito subjekto duomenys komisinių mokesčių apskaitos tikslais."
                e.Severity = Validation.RuleSeverity.Error
                Return False
            End If

            Return True
        End Function

        ''' <summary>
        ''' Rule ensuring that the values of the properties 
        ''' related to Bank are valid (for (pseudo) bank account).
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function ManagingPersonValidation(ByVal target As Object, _
        ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As CashAccount = DirectCast(target, CashAccount)

            If ValObj._Type <> CashAccountType.BankAccount AndAlso _
                ValObj._Type <> CashAccountType.PseudoBankAccount Then Return True

            Dim value As PersonInfo = DirectCast(target, CashAccount).ManagingPerson

            If value Is Nothing OrElse Not value.ID > 0 Then
                e.Description = "Banko arba pseudo banko sąskaitai turi būti nurodytas " _
                    & "sąskaitą administruojantis asmuo (bankas, unija, pan.) komisinių " _
                    & "mokesčių apskaitos tikslais."
                e.Severity = Validation.RuleSeverity.Error
                Return False
            End If

            Return True
        End Function

        ''' <summary>
        ''' Rule ensuring that the values of the properties 
        ''' related to Bank are valid (for (pseudo) bank account).
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function BankFeeCostsAccountValidation(ByVal target As Object, _
        ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As CashAccount = DirectCast(target, CashAccount)

            If ValObj._Type <> CashAccountType.BankAccount AndAlso _
                ValObj._Type <> CashAccountType.PseudoBankAccount Then Return True

            Dim value As Long = DirectCast(target, CashAccount).BankFeeCostsAccount

            If Not value > 0 Then
                e.Description = "Banko arba pseudo banko sąskaitai turi būti nurodyta " _
                    & "komisinių mokesčių (sąnaudų) apskaitos sąskaita."
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

        Friend Shared Function NewCashAccount() As CashAccount
            Dim result As New CashAccount
            result.ValidationRules.CheckRules()
            Return result
        End Function

        Friend Shared Function GetCashAccount(ByVal dr As DataRow) As CashAccount
            Return New CashAccount(dr)
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

            _ID = CIntSafe(dr.Item(0), 0)
            _Name = CStrSafe(dr.Item(1)).Trim
            _Account = CLongSafe(dr.Item(2), 0)
            _BankAccountNumber = CStrSafe(dr.Item(3)).Trim
            _BankName = CStrSafe(dr.Item(4)).Trim
            _BankCode = CStrSafe(dr.Item(5)).Trim
            _IsLitasEsisCompliant = ConvertDbBoolean(CIntSafe(dr.Item(6), 0))
            _CurrencyCode = CStrSafe(dr.Item(7)).Trim
            _EnforceUniqueOperationID = ConvertDbBoolean(CIntSafe(dr.Item(8), 0))
            _BankFeeLimit = CIntSafe(dr.Item(9), 0)
            _Type = ConvertEnumDatabaseCode(Of CashAccountType)(CIntSafe(dr.Item(10), 0))
            _BalanceAtBegining = CDblSafe(dr.Item(11), 0)
            _IsObsolete = ConvertDbBoolean(CIntSafe(dr.Item(12), 0))
            _BankFeeCostsAccount = CLongSafe(dr.Item(13), 0)
            _IsInUse = ConvertDbBoolean(CIntSafe(dr.Item(14), 0))
            _ManagingPerson = PersonInfo.GetPersonInfo(dr, 15)

            MarkOld()

            ValidationRules.CheckRules()

        End Sub

        Friend Sub Insert(ByVal parent As CashAccountList)

            Dim myComm As New SQLCommand("InsertCashAccount")
            AddWithParams(myComm)
            myComm.AddParam("?AH", ConvertEnumDatabaseCode(_Type))

            myComm.Execute()

            _ID = Convert.ToInt32(myComm.LastInsertID)

            MarkOld()

        End Sub

        Friend Sub Update(ByVal parent As CashAccountList)

            Dim myComm As New SQLCommand("UpdateCashAccount")
            myComm.AddParam("?CD", _ID)
            AddWithParams(myComm)

            myComm.Execute()

            MarkOld()

        End Sub

        Friend Sub DeleteSelf()

            Dim myComm As New SQLCommand("DeleteCashAccount")
            myComm.AddParam("?CD", _ID)

            myComm.Execute()

            MarkNew()

        End Sub

        Private Sub AddWithParams(ByRef myComm As SQLCommand)
            myComm.AddParam("?AA", _Name.Trim)
            myComm.AddParam("?AB", _BankName.Trim)
            myComm.AddParam("?AC", _BankCode.Trim)
            myComm.AddParam("?AD", ConvertDbBoolean(_IsLitasEsisCompliant))
            myComm.AddParam("?AE", _CurrencyCode.Trim)
            myComm.AddParam("?AF", ConvertDbBoolean(_EnforceUniqueOperationID))
            myComm.AddParam("?AG", _BankFeeLimit)
            myComm.AddParam("?AI", CRound(_BalanceAtBegining))
            myComm.AddParam("?AJ", ConvertDbBoolean(_IsObsolete))
            myComm.AddParam("?AK", _Account)
            myComm.AddParam("?AL", _BankAccountNumber.Trim)
            myComm.AddParam("?AM", _BankFeeCostsAccount)
            If Not _ManagingPerson Is Nothing AndAlso _ManagingPerson.ID > 0 Then
                myComm.AddParam("?AN", _ManagingPerson.ID)
            Else
                myComm.AddParam("?AN", 0)
            End If
        End Sub

        Friend Sub CheckIfInUse()

            If IsNew Then Exit Sub

            Dim myComm As New SQLCommand("CheckIfCashAccountUsed")
            myComm.AddParam("?CD", _ID)

            Using myData As DataTable = myComm.Fetch
                If myData.Rows.Count < 1 Then
                    _IsInUse = False
                Else
                    _IsInUse = ConvertDbBoolean(CIntSafe(myData.Rows(0).Item(0), 0))
                End If
            End Using

        End Sub

#End Region

    End Class

End Namespace