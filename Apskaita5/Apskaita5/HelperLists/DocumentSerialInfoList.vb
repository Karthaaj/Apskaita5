Namespace HelperLists

    <Serializable()> _
    Public Class DocumentSerialInfoList
        Inherits ReadOnlyListBase(Of DocumentSerialInfoList, DocumentSerialInfo)

#Region " Business Methods "

#End Region

#Region " Authorization Rules "

        Public Shared Function CanGetObject() As Boolean
            Return ApplicationContext.User.IsInRole("HelperLists.DocumentSerialInfoList1")
        End Function

#End Region

#Region " Factory Methods "

        Public Shared Function GetList() As DocumentSerialInfoList

            Dim result As DocumentSerialInfoList = DirectCast(CacheManager.GetItemFromCache( _
                GetType(DocumentSerialInfoList), GetType(DocumentSerialInfoList), Nothing), _
                DocumentSerialInfoList)

            If result Is Nothing Then
                result = DataPortal.Fetch(Of DocumentSerialInfoList)(New Criteria())
                CacheManager.AddCacheItem(GetType(DocumentSerialInfoList), result, Nothing)
            End If

            Return result

        End Function

        Public Shared Function GetCachedFilteredList(ByVal ShowEmpty As Boolean, _
            ByVal ForDocumentType As Settings.DocumentSerialType) _
            As Csla.FilteredBindingList(Of DocumentSerialInfo)

            Dim FilterToApply(1) As Object
            FilterToApply(0) = ConvertDbBoolean(ShowEmpty)
            FilterToApply(1) = ConvertEnumDatabaseCode(ForDocumentType)

            Dim result As Csla.FilteredBindingList(Of DocumentSerialInfo) = _
                DirectCast(CacheManager.GetItemFromCache(GetType(DocumentSerialInfoList), _
                GetType(Csla.FilteredBindingList(Of DocumentSerialInfo)), FilterToApply), _
                Csla.FilteredBindingList(Of DocumentSerialInfo))

            If result Is Nothing Then

                Dim BaseList As DocumentSerialInfoList = DocumentSerialInfoList.GetList
                result = New Csla.FilteredBindingList(Of DocumentSerialInfo) _
                    (BaseList, AddressOf DocumentSerialInfoFilter)
                result.ApplyFilter("", FilterToApply)
                CacheManager.AddCacheItem(GetType(DocumentSerialInfoList), result, FilterToApply)

            End If

            Return result

        End Function

        Public Shared Sub InvalidateCache()
            CacheManager.InvalidateCache(GetType(DocumentSerialInfoList))
        End Sub

        Public Shared Function CacheIsInvalidated() As Boolean
            Return CacheManager.CacheIsInvalidated(GetType(DocumentSerialInfoList))
        End Function

        Private Shared Function DocumentSerialInfoFilter(ByVal item As Object, _
            ByVal filterValue As Object) As Boolean

            If filterValue Is Nothing OrElse DirectCast(filterValue, Object()).Length < 2 Then Return True

            Dim ShowEmpty As Boolean = ConvertDbBoolean( _
                DirectCast(DirectCast(filterValue, Object())(0), Integer))
            Dim ForDocumentType As Settings.DocumentSerialType = _
                ConvertEnumDatabaseCode(Of Settings.DocumentSerialType) _
                (DirectCast(DirectCast(filterValue, Object())(1), Integer))

            Dim CI As DocumentSerialInfo = DirectCast(item, DocumentSerialInfo)

            If Not ShowEmpty AndAlso Not CI.ID > 0 Then Return False
            If CI.ID > 0 AndAlso CI.DocumentType <> ForDocumentType Then Return False

            Return True

        End Function

        Private Shared Function IsApplicationWideCache() As Boolean
            Return False
        End Function

        Private Shared Function GetListOnServer() As DocumentSerialInfoList
            Dim result As New DocumentSerialInfoList
            result.DataPortal_Fetch(New Criteria)
            Return result
        End Function


        Private Sub New()
            ' require use of factory methods
        End Sub

#End Region

#Region " Data Access "

        <Serializable()> _
        Private Class Criteria
            Public Sub New()
            End Sub
        End Class

        Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

            If Not CanGetObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka šiems duomenims gauti.")

            Dim myComm As New SQLCommand("FetchDocumentSerialInfoList")

            Using myData As DataTable = myComm.Fetch

                RaiseListChangedEvents = False
                IsReadOnly = False

                For Each dr As DataRow In myData.Rows
                    Add(DocumentSerialInfo.GetDocumentSerialInfo(dr))
                Next

                IsReadOnly = True
                RaiseListChangedEvents = True

            End Using

        End Sub

#End Region

    End Class

End Namespace