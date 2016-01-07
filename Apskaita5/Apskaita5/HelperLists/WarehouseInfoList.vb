Namespace HelperLists

    ''' <summary>
    ''' Represents a list of <see cref="Goods.Warehouse">goods warehouse</see> value objects.
    ''' </summary>
    ''' <remarks>Exists a single instance per company.</remarks>
    <Serializable()> _
    Public Class WarehouseInfoList
        Inherits ReadOnlyListBase(Of WarehouseInfoList, WarehouseInfo)

#Region " Business Methods "

        ''' <summary>
        ''' Gets a warehouse info instance by the warehouse ID.
        ''' Returns null if there is no warehouse with the id value requested.
        ''' </summary>
        ''' <param name="id">a <see cref="Goods.Warehouse.ID">warehouse id</see> to look for</param>
        ''' <remarks></remarks>
        Public Function GetItem(ByVal id As Integer) As WarehouseInfo
            If Not id > 0 Then Return Nothing
            For Each i As WarehouseInfo In Me
                If i.ID = id Then Return i
            Next
            Return Nothing
        End Function

        ''' <summary>
        ''' Gets a warehouse info instance by the warehouse name.
        ''' Returns null if there is no warehouse with the name requested.
        ''' </summary>
        ''' <param name="name">a <see cref="Goods.Warehouse.Name">warehouse name</see> to look for</param>
        ''' <remarks></remarks>
        Public Function GetItem(ByVal name As String) As WarehouseInfo
            If StringIsNullOrEmpty(name) Then Return Nothing
            For Each i As WarehouseInfo In Me
                If i.Name.Trim.ToLower = name.Trim.ToLower Then Return i
            Next
            Return Nothing
        End Function

#End Region

#Region " Authorization Rules "

        Public Shared Function CanGetObject() As Boolean
            Return ApplicationContext.User.IsInRole("HelperLists.WarehouseInfoList1")
        End Function

#End Region

#Region " Factory Methods "

        ''' <summary>
        ''' Gets a current goods warehouse info value object list from database.
        ''' </summary>
        ''' <remarks>Result is cached.
        ''' Required by <see cref="AccDataAccessLayer.CacheManager">AccDataAccessLayer.CacheManager</see>.</remarks>
        Public Shared Function GetList() As WarehouseInfoList

            Dim result As WarehouseInfoList = CacheManager.GetItemFromCache(Of WarehouseInfoList)( _
                GetType(WarehouseInfoList), Nothing)

            If result Is Nothing Then
                result = DataPortal.Fetch(Of WarehouseInfoList)(New Criteria())
                CacheManager.AddCacheItem(GetType(WarehouseInfoList), result, Nothing)
            End If

            Return result

        End Function

        ''' <summary>
        ''' Gets a filtered view of the current goods warehouse info value object list.
        ''' </summary>
        ''' <param name="showEmpty">Wheather to include a placeholder object.</param>
        ''' <param name="showObsolete">Wheather to include warehouses that are obsolete (no loger in use).</param>
        ''' <remarks>Result is cached.
        ''' Required by <see cref="AccDataAccessLayer.CacheManager">AccDataAccessLayer.CacheManager</see>.</remarks>
        Public Shared Function GetCachedFilteredList(ByVal showEmpty As Boolean, _
            ByVal showObsolete As Boolean) As Csla.FilteredBindingList(Of WarehouseInfo)

            Dim filterToApply(1) As Object
            filterToApply(0) = ConvertDbBoolean(showEmpty)
            filterToApply(1) = ConvertDbBoolean(showObsolete)

            Dim result As Csla.FilteredBindingList(Of WarehouseInfo) = _
                CacheManager.GetItemFromCache(Of Csla.FilteredBindingList(Of WarehouseInfo)) _
                (GetType(WarehouseInfoList), filterToApply)

            If result Is Nothing Then

                Dim baseList As WarehouseInfoList = WarehouseInfoList.GetList
                result = New Csla.FilteredBindingList(Of WarehouseInfo)(baseList, _
                    AddressOf WarehouseInfoListFilter)
                result.ApplyFilter("", filterToApply)
                CacheManager.AddCacheItem(GetType(WarehouseInfoList), result, filterToApply)

            End If

            Return result

        End Function

        ''' <summary>
        ''' Invalidates the current goods warehouse info value object list cache 
        ''' so that the next <see cref="GetList">GetList</see> call would hit the database.
        ''' </summary>
        ''' <remarks>Required by <see cref="AccDataAccessLayer.CacheManager">AccDataAccessLayer.CacheManager</see>.</remarks>
        Public Shared Sub InvalidateCache()
            CacheManager.InvalidateCache(GetType(WarehouseInfoList))
        End Sub

        ''' <summary>
        ''' Returnes true if the cache does not contain a current goods warehouse 
        ''' info value object list.
        ''' </summary>
        ''' <remarks>Required by <see cref="AccDataAccessLayer.CacheManager">AccDataAccessLayer.CacheManager</see>.</remarks>
        Public Shared Function CacheIsInvalidated() As Boolean
            Return CacheManager.CacheIsInvalidated(GetType(WarehouseInfoList))
        End Function

        ''' <summary>
        ''' Returns true if the collection is common across all the databases.
        ''' I.e. cache is not to be cleared on changing databases.
        ''' </summary>
        ''' <remarks>Required by <see cref="AccDataAccessLayer.CacheManager">AccDataAccessLayer.CacheManager</see>.</remarks>
        Private Shared Function IsApplicationWideCache() As Boolean
            Return False
        End Function

        ''' <summary>
        ''' Gets a current goods warehouse info value object list from database bypassing dataportal.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Should only be called server side.
        ''' Required by <see cref="AccDataAccessLayer.CacheManager">AccDataAccessLayer.CacheManager</see>.</remarks>
        Private Shared Function GetListOnServer() As WarehouseInfoList
            Dim result As New WarehouseInfoList
            result.DataPortal_Fetch(New Criteria)
            Return result
        End Function

        ''' <summary>
        ''' Gets a current goods warehouse info value object list from database 
        ''' bypassing dataportal.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Should only be called server side.</remarks>
        Friend Shared Function GetListChild() As WarehouseInfoList
            Dim result As New WarehouseInfoList
            result.DoFetch()
            Return result
        End Function


        Private Shared Function WarehouseInfoListFilter(ByVal item As Object, _
                    ByVal filterValue As Object) As Boolean

            If filterValue Is Nothing OrElse Not TypeOf filterValue Is Object() _
                OrElse Not DirectCast(filterValue, Object()).Length > 0 Then Return True

            Dim showEmpty As Boolean = ConvertDbBoolean( _
                DirectCast(DirectCast(filterValue, Object())(0), Integer))
            Dim showObsolete As Boolean = ConvertDbBoolean( _
                DirectCast(DirectCast(filterValue, Object())(1), Integer))

            ' no criteria to apply
            If showEmpty AndAlso showObsolete Then Return True

            Dim w As WarehouseInfo = DirectCast(item, WarehouseInfo)

            If Not showEmpty AndAlso Not w.ID > 0 Then Return False
            If Not showObsolete AndAlso w.ID > 0 AndAlso w.IsObsolete Then Return False

            Return True

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
                My.Resources.Common_SecuritySelectDenied)

            DoFetch()

        End Sub

        Private Sub DoFetch()

            Dim myComm As New SQLCommand("FetchWarehouseInfoList")

            Using myData As DataTable = myComm.Fetch

                RaiseListChangedEvents = False
                IsReadOnly = False

                Add(WarehouseInfo.NewWarehouseInfo)

                For Each dr As DataRow In myData.Rows
                    Add(WarehouseInfo.GetWarehouseInfo(dr, 0))
                Next

                IsReadOnly = True
                RaiseListChangedEvents = True

            End Using

        End Sub

#End Region

    End Class

End Namespace