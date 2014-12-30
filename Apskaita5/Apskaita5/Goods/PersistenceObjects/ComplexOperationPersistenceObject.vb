Namespace Goods

    <Serializable()> _
    Friend Class ComplexOperationPersistenceObject
        
#Region " Business Methods "

        Private _ID As Integer = 0
        Private _OperationDate As Date = Today
        Private _OperationType As GoodsComplexOperationType = GoodsComplexOperationType.InternalTransfer
        Private _JournalEntryID As Integer = 0
        Private _DocNo As String = ""
        Private _Content As String = ""
        Private _GoodsID As Integer = 0
        Private _AccountOperation As Long = 0
        Private _WarehouseID As Integer = 0
        Private _Warehouse As WarehouseInfo = Nothing
        Private _SecondaryWarehouseID As Integer = 0
        Private _SecondaryWarehouse As WarehouseInfo = Nothing
        Private _InsertDate As DateTime = Now
        Private _UpdateDate As DateTime = Now


        Public ReadOnly Property ID() As Integer
            Get
                Return _ID
            End Get
        End Property

        Public Property OperationDate() As Date
            Get
                Return _OperationDate
            End Get
            Set(ByVal value As Date)
                If _OperationDate.Date <> value.Date Then
                    _OperationDate = value.Date
                End If
            End Set
        End Property

        Public Property OperationType() As GoodsComplexOperationType
            Get
                Return _OperationType
            End Get
            Set(ByVal value As GoodsComplexOperationType)
                If _OperationType <> value Then
                    _OperationType = value
                End If
            End Set
        End Property

        Public Property JournalEntryID() As Integer
            Get
                Return _JournalEntryID
            End Get
            Set(ByVal value As Integer)
                If _JournalEntryID <> value Then
                    _JournalEntryID = value
                End If
            End Set
        End Property

        Public Property DocNo() As String
            Get
                Return _DocNo.Trim
            End Get
            Set(ByVal value As String)
                If value Is Nothing Then value = ""
                If _DocNo.Trim <> value.Trim Then
                    _DocNo = value.Trim
                End If
            End Set
        End Property

        Public Property Content() As String
            Get
                Return _Content.Trim
            End Get
            Set(ByVal value As String)
                If value Is Nothing Then value = ""
                If _Content.Trim <> value.Trim Then
                    _Content = value.Trim
                End If
            End Set
        End Property

        Public Property GoodsID() As Integer
            Get
                Return _GoodsID
            End Get
            Set(ByVal value As Integer)
                If _GoodsID <> value Then
                    _GoodsID = value
                End If
            End Set
        End Property

        Public Property AccountOperation() As Long
            Get
                Return _AccountOperation
            End Get
            Set(ByVal value As Long)
                If _AccountOperation <> value Then
                    _AccountOperation = value
                End If
            End Set
        End Property

        Public Property WarehouseID() As Integer
            Get
                Return _WarehouseID
            End Get
            Set(ByVal value As Integer)
                If _WarehouseID <> value Then
                    _WarehouseID = value
                End If
            End Set
        End Property

        Public Property Warehouse() As WarehouseInfo
            Get
                Return _Warehouse
            End Get
            Set(ByVal value As WarehouseInfo)
                If Not (_Warehouse Is Nothing AndAlso value Is Nothing) _
                    AndAlso Not (Not _Warehouse Is Nothing AndAlso Not value Is Nothing _
                    AndAlso _Warehouse.ID = value.ID) Then
                    _Warehouse = value
                    If Not _Warehouse Is Nothing Then
                        _WarehouseID = _Warehouse.ID
                    Else
                        _WarehouseID = 0
                    End If
                End If
            End Set
        End Property

        Public Property SecondaryWarehouseID() As Integer
            Get
                Return _SecondaryWarehouseID
            End Get
            Set(ByVal value As Integer)
                If _SecondaryWarehouseID <> value Then
                    _SecondaryWarehouseID = value
                End If
            End Set
        End Property

        Public Property SecondaryWarehouse() As WarehouseInfo
            Get
                Return _SecondaryWarehouse
            End Get
            Set(ByVal value As WarehouseInfo)
                If Not (_SecondaryWarehouse Is Nothing AndAlso value Is Nothing) _
                    AndAlso Not (Not _SecondaryWarehouse Is Nothing AndAlso Not value Is Nothing _
                    AndAlso _SecondaryWarehouse.ID = value.ID) Then
                    _SecondaryWarehouse = value
                    If Not _SecondaryWarehouse Is Nothing Then
                        _SecondaryWarehouseID = _SecondaryWarehouse.ID
                    Else
                        _SecondaryWarehouseID = 0
                    End If
                End If
            End Set
        End Property

        Public ReadOnly Property InsertDate() As DateTime

            Get
                Return _InsertDate
            End Get
        End Property

        Public ReadOnly Property UpdateDate() As DateTime

            Get
                Return _UpdateDate
            End Get
        End Property

#End Region

#Region " Factory Methods "

        Public Shared Function NewComplexOperationPersistenceObject() As ComplexOperationPersistenceObject
            Return New ComplexOperationPersistenceObject
        End Function

        Public Shared Function GetComplexOperationPersistenceObject(ByVal OperationID As Integer, _
            ByVal DoFetch As Boolean) As ComplexOperationPersistenceObject
            If Not OperationID > 0 Then Throw New Exception("Klaida. Nenurodytas operacijos ID.")
            Return New ComplexOperationPersistenceObject(OperationID, DoFetch)
        End Function


        Private Sub New()
            ' require use of factory methods
        End Sub

        Private Sub New(ByVal OperationID As Integer, ByVal DoFetch As Boolean)
            _ID = OperationID
            If DoFetch Then Fetch(OperationID)
        End Sub

#End Region

#Region " Data Access "

        Private Sub Fetch(ByVal OperationID As Integer)

            Dim myComm As New SQLCommand("FetchComplexOperationPersistenceObject")
            myComm.AddParam("?OD", OperationID)

            Using myData As DataTable = myComm.Fetch

                If myData.Rows.Count < 1 Then Throw New Exception("Klaida. Kompleksinė " _
                    & "prekių operacija, kurios ID=" & OperationID.ToString & ", nerasta.")

                Dim dr As DataRow = myData.Rows(0)

                _ID = CIntSafe(dr.Item(0), 0)
                _OperationDate = CDateSafe(dr.Item(1), Today)
                _JournalEntryID = CIntSafe(dr.Item(2), 0)
                _DocNo = CStrSafe(dr.Item(3)).Trim
                _Content = CStrSafe(dr.Item(4)).Trim
                _GoodsID = CIntSafe(dr.Item(5), 0)
                _OperationType = ConvertEnumDatabaseCode(Of GoodsComplexOperationType)(CIntSafe(dr.Item(6)))
                _AccountOperation = CLongSafe(dr.Item(7), 0)
                _InsertDate = DateTime.SpecifyKind(CDateTimeSafe(dr.Item(8), Date.UtcNow), _
                    DateTimeKind.Utc).ToLocalTime
                _UpdateDate = DateTime.SpecifyKind(CDateTimeSafe(dr.Item(9), Date.UtcNow), _
                    DateTimeKind.Utc).ToLocalTime
                _Warehouse = WarehouseInfo.GetWarehouseInfo(dr, 10)
                _SecondaryWarehouse = WarehouseInfo.GetWarehouseInfo(dr, 15)
                If Not _Warehouse Is Nothing Then _WarehouseID = _Warehouse.ID
                If Not _SecondaryWarehouse Is Nothing Then _SecondaryWarehouseID = _SecondaryWarehouse.ID

            End Using

        End Sub


        Public Function Save(ByVal UpdateFinancialData As Boolean, _
            ByVal UpdateWarehouse As Boolean, ByVal UpdateSecondaryWarehouse As Boolean) As Integer

            Dim myComm As SQLCommand

            If _ID > 0 Then

                If UpdateFinancialData Then
                    myComm = New SQLCommand("UpdateComplexOperationPersistenceObjectFull")
                    AddWithParamsFinancial(myComm)
                ElseIf UpdateWarehouse Then
                    myComm = New SQLCommand("UpdateComplexOperationPersistenceObjectGeneral2")
                    myComm.AddParam("?AG", _WarehouseID)
                ElseIf UpdateSecondaryWarehouse Then
                    myComm = New SQLCommand("UpdateComplexOperationPersistenceObjectGeneral3")
                    myComm.AddParam("?AH", _SecondaryWarehouseID)
                Else
                    myComm = New SQLCommand("UpdateComplexOperationPersistenceObjectGeneral")
                End If
                AddWithParamsGeneral(myComm)
                myComm.AddParam("?CD", _ID)

            Else

                myComm = New SQLCommand("InsertComplexOperationPersistenceObject")
                AddWithParamsGeneral(myComm)
                AddWithParamsFinancial(myComm)
                myComm.AddParam("?AE", _GoodsID)
                myComm.AddParam("?AF", ConvertEnumDatabaseCode(_OperationType))

            End If

            myComm.Execute()

            If Not _ID > 0 Then _ID = Convert.ToInt32(myComm.LastInsertID)

            Return _ID

        End Function

        Private Sub AddWithParamsGeneral(ByRef myComm As SQLCommand)

            _UpdateDate = DateTime.Now
            _UpdateDate = New DateTime(Convert.ToInt64(Math.Floor(_UpdateDate.Ticks / TimeSpan.TicksPerSecond) _
                * TimeSpan.TicksPerSecond))
            If Not _ID > 0 Then _InsertDate = _UpdateDate

            myComm.AddParam("?AA", _OperationDate.Date)
            myComm.AddParam("?AC", _DocNo.Trim)
            myComm.AddParam("?AD", _Content.Trim)
            myComm.AddParam("?AI", _UpdateDate)

        End Sub

        Private Sub AddWithParamsFinancial(ByRef myComm As SQLCommand)

            myComm.AddParam("?AB", _JournalEntryID)
            myComm.AddParam("?AG", _WarehouseID)
            myComm.AddParam("?AH", _SecondaryWarehouseID)
            myComm.AddParam("?AJ", _AccountOperation)

        End Sub


        Public Shared Sub Delete(ByVal OperationID As Integer)

            Dim myComm As New SQLCommand("DeleteComplexOperationPersistenceObject")
            myComm.AddParam("?CD", OperationID)

            myComm.Execute()

        End Sub

        Public Shared Sub DeleteOperations(ByVal OperationID As Integer)

            Dim myComm As New SQLCommand("DeleteOperationsByComplexParent")
            myComm.AddParam("?CD", OperationID)

            myComm.Execute()

        End Sub

        Public Shared Sub DeleteConsignments(ByVal OperationID As Integer)

            Dim myComm As New SQLCommand("DeleteConsignmentsByComplexParent")
            myComm.AddParam("?OD", OperationID)

            myComm.Execute()

        End Sub

        Public Shared Sub DeleteConsignmentDiscards(ByVal OperationID As Integer)

            Dim myComm As New SQLCommand("DeleteConsignmentDiscardsByComplexParent")
            myComm.AddParam("?OD", OperationID)

            myComm.Execute()

        End Sub


        Public Shared Sub CheckIfUpdateDateChanged(ByVal OperationID As Integer, _
            ByVal CurrentUpdateDate As DateTime)

            Dim myComm As New SQLCommand("CheckIfGoodsComplexOperationUpdateDateChanged")
            myComm.AddParam("?CD", OperationID)

            Using myData As DataTable = myComm.Fetch
                If myData.Rows.Count < 1 OrElse CDateTimeSafe(myData.Rows(0).Item(0), _
                    Date.MinValue) = Date.MinValue Then Throw New Exception( _
                    "Klaida. Prekių operacija, kurios ID=" & OperationID.ToString & ", nerasta.")
                If DateTime.SpecifyKind(CDateTimeSafe(myData.Rows(0).Item(0), DateTime.UtcNow), _
                    DateTimeKind.Utc).ToLocalTime <> CurrentUpdateDate Then Throw New Exception( _
                    "Klaida. Prekių operacijos atnaujinimo data pasikeitė. Teigtina, kad kitas " _
                    & "vartotojas redagavo šį objektą.")
            End Using

        End Sub

#End Region

    End Class

End Namespace