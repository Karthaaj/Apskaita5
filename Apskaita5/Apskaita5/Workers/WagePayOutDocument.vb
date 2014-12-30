Namespace Workers

    <Serializable()> _
    Public Class WagePayOutDocument
        Inherits BusinessBase(Of WagePayOutDocument)
        Implements IIsDirtyEnough

#Region " Business Methods "

        Private _ID As Integer
        Private _Type As DocumentType
        Private _TypeHumanReadable As String
        Private _Date As Date
        Private _Year As Integer
        Private _Month As Integer
        Private _Number As Integer
        Private _UpdateDate As DateTime = Now
        Private WithEvents _Items As WagePayOutItemList


        Private SuspendChildListChangedEvents As Boolean = False
        ' used to implement automatic sort in datagridview
        <NotUndoable()> _
        <NonSerialized()> _
        Private _ItemsSortedList As Csla.SortedBindingList(Of WagePayOutItem) = Nothing

        Public ReadOnly Property ID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ID
            End Get
        End Property

        Public ReadOnly Property UpdateDate() As DateTime
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _UpdateDate
            End Get
        End Property

        Public ReadOnly Property [Type]() As DocumentType
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Type
            End Get
        End Property

        Public ReadOnly Property TypeHumanReadable() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _TypeHumanReadable
            End Get
        End Property

        Public ReadOnly Property [Date]() As Date
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Date
            End Get
        End Property

        Public ReadOnly Property Year() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Year
            End Get
        End Property

        Public ReadOnly Property Month() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Month
            End Get
        End Property

        Public ReadOnly Property Number() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Number
            End Get
        End Property

        Public ReadOnly Property Items() As WagePayOutItemList
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Items
            End Get
        End Property

        Public ReadOnly Property ItemsSorted() As Csla.SortedBindingList(Of WagePayOutItem)
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                If _ItemsSortedList Is Nothing Then _ItemsSortedList = _
                    New Csla.SortedBindingList(Of WagePayOutItem)(_Items)
                Return _ItemsSortedList
            End Get
        End Property

        Public ReadOnly Property IsDirtyEnough() As Boolean _
            Implements IIsDirtyEnough.IsDirtyEnough
            Get
                If Not IsNew Then Return IsDirty
                Return _Items.GetIfAnyItemIsChecked
            End Get
        End Property

        Public Overrides ReadOnly Property IsDirty() As Boolean
            Get
                Return MyBase.IsDirty OrElse _Items.IsDirty
            End Get
        End Property

        Public Overrides ReadOnly Property IsValid() As Boolean
            Get
                Return MyBase.IsValid AndAlso _Items.IsValid
            End Get
        End Property


        Public Overrides Function Save() As WagePayOutDocument

            Me.ValidationRules.CheckRules()
            If Not IsValid Then Throw New Exception("Duomenyse yra klaidų: " & vbCrLf _
                & Me.GetAllBrokenRules)

            Return MyBase.Save

        End Function


        Private Sub Items_Changed(ByVal sender As Object, _
            ByVal e As System.ComponentModel.ListChangedEventArgs) Handles _Items.ListChanged

            If SuspendChildListChangedEvents Then Exit Sub

        End Sub

        ''' <summary>
        ''' Helper method. Takes care of child lists loosing their handlers.
        ''' </summary>
        Protected Overrides Function GetClone() As Object
            Dim result As WagePayOutDocument = DirectCast(MyBase.GetClone(), WagePayOutDocument)
            result.RestoreChildListsHandles()
            Return result
        End Function

        Protected Overrides Sub OnDeserialized(ByVal context As System.Runtime.Serialization.StreamingContext)
            MyBase.OnDeserialized(context)
            RestoreChildListsHandles()
        End Sub

        Protected Overrides Sub UndoChangesComplete()
            MyBase.UndoChangesComplete()
            RestoreChildListsHandles()
        End Sub

        ''' <summary>
        ''' Helper method. Takes care of TaskTimeSpans loosing its handler. See GetClone method.
        ''' </summary>
        Friend Sub RestoreChildListsHandles()
            Try
                RemoveHandler _Items.ListChanged, AddressOf Items_Changed
            Catch ex As Exception
            End Try
            AddHandler _Items.ListChanged, AddressOf Items_Changed
        End Sub


        Public Function GetAllBrokenRules() As String
            Dim result As String = ""
            If Not MyBase.IsValid Then result = AddWithNewLine(result, _
                Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Error), False)
            If Not _Items.IsValid Then result = AddWithNewLine(result, _
                _Items.GetAllBrokenRules, False)
            Return result
        End Function

        Public Function GetAllWarnings() As String
            Dim result As String = ""
            If Not MyBase.BrokenRulesCollection.WarningCount > 0 Then result = AddWithNewLine(result, _
                Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Warning), False)
            result = AddWithNewLine(result, _Items.GetAllWarnings, False)
            Return result
        End Function


        Protected Overrides Function GetIdValue() As Object
            Return _ID
        End Function

        Public Overrides Function ToString() As String
            If Not _ID > 0 Then Return ""
            Return _Date.ToShortDateString & " " & _TypeHumanReadable & " Nr. " & _Number
        End Function

#End Region

#Region " Validation Rules "

        Protected Overrides Sub AddBusinessRules()

        End Sub

#End Region

#Region " Authorization Rules "

        Protected Overrides Sub AddAuthorizationRules()
            AuthorizationRules.AllowWrite("Workers.WagePayOut3")
        End Sub

        Public Shared Function CanAddObject() As Boolean
            Return False
        End Function

        Public Shared Function CanGetObject() As Boolean
            Return ApplicationContext.User.IsInRole("Workers.WagePayOut1")
        End Function

        Public Shared Function CanEditObject() As Boolean
            Return ApplicationContext.User.IsInRole("Workers.WagePayOut3")
        End Function

        Public Shared Function CanDeleteObject() As Boolean
            Return False
        End Function

#End Region

#Region " Factory Methods "

        Public Shared Function GetWagePayOutDocument(ByVal nID As Integer) As WagePayOutDocument

            Return DataPortal.Fetch(Of WagePayOutDocument)(New Criteria(nID))

        End Function

        Private Sub New()
            ' require use of factory methods
        End Sub

#End Region

#Region " Data Access "

        <Serializable()> _
        Private Class Criteria
            Private _ID As Integer
            Public ReadOnly Property ID() As Integer
                Get
                    Return _ID
                End Get
            End Property
            Public Sub New(ByVal nID As Integer)
                _ID = nID
            End Sub
        End Class

        Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

            If Not CanGetObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka duomenims gauti.")

            Dim myComm As New SQLCommand("FetchWagePayOutDocumentGeneral")
            myComm.AddParam("?SD", criteria.ID)

            Using myData As DataTable = myComm.Fetch

                If Not myData.Rows.Count > 0 Then Throw New Exception( _
                    "Klaida. Nepavyko rasti žiniaraščio duomenų pagal ID=" _
                        & criteria.ID.ToString & ".")

                Dim dr As DataRow = myData.Rows(0)

                If CStrSafe(dr.Item(0)).Trim.ToLower = "d" Then
                    _Type = DocumentType.WageSheet
                Else
                    _Type = DocumentType.ImprestSheet
                End If
                _TypeHumanReadable = ConvertEnumHumanReadable(_Type)

                _Date = CDateSafe(dr.Item(1), Today)
                _Year = CIntSafe(dr.Item(2))
                _Month = CIntSafe(dr.Item(3))
                _Number = CIntSafe(dr.Item(4))
                _UpdateDate = DateTime.SpecifyKind(CDateTimeSafe(dr.Item(5), Date.UtcNow), _
                    DateTimeKind.Utc).ToLocalTime

                _ID = criteria.ID

            End Using

            If _Type = DocumentType.ImprestSheet Then
                myComm = New SQLCommand("FetchWagePayOutDocumentDetails1")
            Else
                myComm = New SQLCommand("FetchWagePayOutDocumentDetails2")
            End If
            myComm.AddParam("?SD", criteria.ID)

            Using myData As DataTable = myComm.Fetch
                _Items = WagePayOutItemList.GetWagePayOutItemList(myData, _Date)
            End Using

            MarkOld()

        End Sub

        Protected Overrides Sub DataPortal_Update()

            If IsNew AndAlso Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka naujiems duomenims įvesti.")
            If Not IsNew AndAlso Not CanEditObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka duomenims pakeisti.")

            CheckIfUpdateDateChanged()

            _UpdateDate = DateTime.Now
            _UpdateDate = New DateTime(Convert.ToInt64(Math.Floor(_UpdateDate.Ticks / TimeSpan.TicksPerSecond) _
                * TimeSpan.TicksPerSecond))

            DatabaseAccess.TransactionBegin()

            _Items.Update(Me)

            Dim myComm As SQLCommand
            If _Type = DocumentType.ImprestSheet Then
                myComm = New SQLCommand("UpdateImprestSheetUpdateDate")
            Else
                myComm = New SQLCommand("UpdateWageSheetUpdateDate")
            End If
            myComm.AddParam("?UD", _UpdateDate.ToUniversalTime)

            DatabaseAccess.TransactionCommit()

            MarkOld()

        End Sub

        Private Sub CheckIfUpdateDateChanged()

            Dim myComm As SQLCommand
            If _Type = DocumentType.ImprestSheet Then
                myComm = New SQLCommand("CheckIfImprestSheetUpdateDateChanged")
            Else
                myComm = New SQLCommand("CheckIfWageSheetUpdateDateChanged")
            End If
            myComm.AddParam("?CD", _ID)

            Using myData As DataTable = myComm.Fetch
                If myData.Rows.Count < 1 OrElse CDateTimeSafe(myData.Rows(0).Item(0), _
                    Date.MinValue) = Date.MinValue Then Throw New Exception( _
                    "Klaida. Objektas, kurio ID=" & _ID.ToString & ", nerastas.")
                If DateTime.SpecifyKind(CDateTimeSafe(myData.Rows(0).Item(0), DateTime.UtcNow), _
                    DateTimeKind.Utc).ToLocalTime <> _UpdateDate Then Throw New Exception( _
                    "Klaida. Dokumento atnaujinimo data pasikeitė. Teigtina, kad kitas " _
                    & "vartotojas redagavo šį objektą.")
            End Using

        End Sub

#End Region

    End Class

End Namespace