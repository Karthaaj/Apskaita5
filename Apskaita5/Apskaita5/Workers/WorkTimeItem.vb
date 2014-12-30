Namespace Workers

    <Serializable()> _
    Public Class WorkTimeItem
        Inherits BusinessBase(Of WorkTimeItem)
        Implements IGetErrorForListItem

#Region " Business Methods "

        Private _Guid As Guid = Guid.NewGuid
        Private _IsChecked As Boolean = True
        Private _ID As Integer = 0
        Private _WorkerID As Integer = 0
        Private _Worker As String = ""
        Private _WorkerPosition As String = ""
        Private _WorkerLoad As Double = 0
        Private _ContractSerial As String = ""
        Private _ContractNumber As Integer = 0
        Private _QuotaDays As Integer = 0
        Private _QuotaHours As Double = 0
        Private _TotalDays As Integer = 0
        Private _TotalHours As Double = 0
        Private _DayList As DayWorkTimeList


        Public ReadOnly Property ID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ID
            End Get
        End Property

        Public ReadOnly Property WorkerID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _WorkerID
            End Get
        End Property

        Public ReadOnly Property Worker() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Worker.Trim
            End Get
        End Property

        Public ReadOnly Property WorkerPosition() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _WorkerPosition.Trim
            End Get
        End Property

        Public ReadOnly Property WorkerLoad() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_WorkerLoad, 3)
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
                If _IsChecked <> value Then
                    _IsChecked = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property QuotaDays() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _QuotaDays
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Integer)
                CanWriteProperty(True)
                If _QuotaDays <> value Then
                    _QuotaDays = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property QuotaHours() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_QuotaHours, ROUNDWORKTIME)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If CRound(_QuotaHours, ROUNDWORKTIME) <> CRound(value, ROUNDWORKTIME) Then
                    _QuotaHours = CRound(value, ROUNDWORKTIME)
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property Day1() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(1).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(1, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType1() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(1).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(1, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day1")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day2() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(2).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(2, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType2() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
             Get
                Return _DayList.GetItemForDay(2).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(2, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day2")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day3() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(3).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(3, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType3() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(3).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(3, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day3")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day4() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(4).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(4, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType4() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(4).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(4, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day4")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day5() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(5).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(5, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType5() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(5).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(5, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day5")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day6() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(6).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(6, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType6() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(6).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(6, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day6")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day7() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(7).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(7, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType7() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(7).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(7, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day7")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day8() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(8).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(8, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType8() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(8).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(8, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day8")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day9() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(9).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(9, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType9() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(9).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(9, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day9")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day10() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(10).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(10, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType10() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(10).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(10, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day10")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day11() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(11).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(11, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType11() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(11).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(11, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day11")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day12() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(12).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(12, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType12() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(12).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(12, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day12")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day13() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(13).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(13, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType13() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(13).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(13, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day13")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day14() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(14).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(14, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType14() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(14).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(14, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day14")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day15() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(15).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(15, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType15() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(15).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(15, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day15")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day16() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(16).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(16, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType16() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(16).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(16, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day16")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day17() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(17).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(17, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType17() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(17).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(17, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day17")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day18() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(18).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(18, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType18() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(18).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(18, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day18")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day19() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(19).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(19, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType19() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(19).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(19, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day19")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day20() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(20).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(20, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType20() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(20).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(20, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day20")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day21() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(21).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(21, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType21() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(21).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(21, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day21")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day22() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(22).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(22, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType22() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(22).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(22, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day22")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day23() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(23).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(23, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType23() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(23).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(23, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day23")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day24() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(24).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(24, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType24() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(24).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(24, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day24")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day25() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(25).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(25, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType25() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(25).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(25, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day25")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day26() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(26).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(26, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType26() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(26).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(26, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day26")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day27() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(27).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(27, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType27() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(27).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(27, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day27")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day28() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(28).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(28, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType28() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(28).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(28, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day28")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day29() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(29).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(29, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType29() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(29).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(29, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day29")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day30() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(30).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(30, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property DayType30() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(30).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(30, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day30")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public Property Day31() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(31).Length
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If _DayList.SetLengthForDay(31, value) Then
                    PropertyHasChanged()
                    RecalculateTotals(True)
                End If
                PropertyHasChanged()
            End Set
        End Property

        Public Property DayType31() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DayList.GetItemForDay(31).Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeClassInfo)
                CanWriteProperty(True)
                If _DayList.SetTypeForDay(31, value) Then
                    PropertyHasChanged()
                    PropertyHasChanged("Day31")
                    RecalculateTotals(True)
                End If
            End Set
        End Property

        Public ReadOnly Property TotalDays() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _TotalDays
            End Get
        End Property

        Public ReadOnly Property TotalHours() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_TotalHours, ROUNDWORKTIME)
            End Get
        End Property



        Public Function GetErrorString() As String _
            Implements IGetErrorForListItem.GetErrorString
            If IsValid Then Return ""
            Return "Klaida (-os) eilutėje '" & Me.ToString & "': " _
                & Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Error)
        End Function

        Public Function GetWarningString() As String _
            Implements IGetErrorForListItem.GetWarningString
            If BrokenRulesCollection.WarningCount < 1 Then Return ""
            Return "Eilutėje '" & Me.ToString & "' gali būti klaida: " _
                & Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Warning)
        End Function


        Private Sub RecalculateTotals(ByVal RaisePropertyHasChanged As Boolean)
            _TotalDays = _DayList.GetTotalDays
            _TotalHours = _DayList.GetTotalHours
            If RaisePropertyHasChanged Then
                PropertyHasChanged("TotalDays")
                PropertyHasChanged("TotalHours")
            End If
        End Sub

        Friend Function GetTotalAbsenceDays(ByVal DefaultRestTimeClass As WorkTimeClassInfo, _
            ByVal DefaultPublicHolidaysClass As WorkTimeClassInfo) As Integer
            Return _DayList.GetTotalAbsenceDays(DefaultRestTimeClass, DefaultPublicHolidaysClass)
        End Function


        Protected Overrides Function GetIdValue() As Object
            Return _Guid
        End Function

        Public Overrides Function ToString() As String
            Return "darbuotojas " & _Worker & ", DS Nr. " & _ContractSerial & _ContractNumber.ToString
        End Function

#End Region

#Region " Validation Rules "

        Protected Overrides Sub AddBusinessRules()

            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
            New CommonValidation.SimpleRuleArgs("QuotaDays", "darbo dienų norma"))
            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
            New CommonValidation.SimpleRuleArgs("QuotaHours", "darbo valandų norma"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day1"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day2"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day3"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day4"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day5"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day6"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day7"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day8"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day9"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day10"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day11"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day12"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day13"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day14"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day15"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day16"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day17"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day18"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day19"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day20"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day21"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day22"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day23"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day24"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day25"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day26"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day27"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day28"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day29"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day30"))
            ValidationRules.AddRule(AddressOf DayValueValidation, New Validation.RuleArgs("Day31"))
            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("TotalDays", "nė viena darbo diena", _
                Validation.RuleSeverity.Warning))
            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("TotalHours", "nė viena darbo valanda", _
                Validation.RuleSeverity.Warning))
        End Sub

        ''' <summary>
        ''' Rule ensuring that the value of property Day n is valid.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function DayValueValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As WorkTimeItem = DirectCast(target, WorkTimeItem)

            Dim DayNumber As Integer = 0
            If Not Integer.TryParse(e.PropertyName.Substring(3), DayNumber) _
                OrElse Not DayNumber > 0 Then Return True
            Dim curDayItem As DayWorkTime = ValObj._DayList.GetItemForDay(DayNumber)
            If curDayItem Is Nothing Then Return True

            If Not curDayItem.IsValid Then
                e.Description = curDayItem.BrokenRulesCollection.ToString(Validation.RuleSeverity.Error)
                e.Severity = Validation.RuleSeverity.Error
                Return False
            ElseIf curDayItem.BrokenRulesCollection.WarningCount > 0 Then
                e.Description = curDayItem.BrokenRulesCollection.ToString(Validation.RuleSeverity.Warning)
                e.Severity = Validation.RuleSeverity.Warning
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

        Friend Shared Function NewWorkTimeItem(ByVal dr As DataRow, _
            ByVal cYear As Integer, ByVal cMonth As Integer, _
            ByVal RestDayInfo As WorkTimeClassInfo, ByVal PublicHolydaysInfo As WorkTimeClassInfo, _
            ByVal cWorkTime As WorkTime) As WorkTimeItem
            Return New WorkTimeItem(dr, cYear, cMonth, RestDayInfo, PublicHolydaysInfo, cWorkTime)
        End Function

        Friend Shared Function GetWorkTimeItem(ByVal dr As DataRow, _
            ByVal DayWorkTimeDataTable As DataTable, ByVal cYear As Integer, _
            ByVal cMonth As Integer) As WorkTimeItem
            Return New WorkTimeItem(dr, DayWorkTimeDataTable, cYear, cMonth)
        End Function

        Private Sub New()
            ' require use of factory methods
            MarkAsChild()
        End Sub

        Private Sub New(ByVal dr As DataRow, ByVal cYear As Integer, _
            ByVal cMonth As Integer, ByVal RestDayInfo As WorkTimeClassInfo, _
            ByVal PublicHolydaysInfo As WorkTimeClassInfo, ByVal cWorkTime As WorkTime)
            MarkAsChild()
            Create(dr, cYear, cMonth, RestDayInfo, PublicHolydaysInfo, cWorkTime)
        End Sub

        Private Sub New(ByVal dr As DataRow, ByVal DayWorkTimeDataTable As DataTable, _
            ByVal cYear As Integer, ByVal cMonth As Integer)
            MarkAsChild()
            Fetch(dr, DayWorkTimeDataTable, cYear, cMonth)
        End Sub

#End Region

#Region " Data Access "

        Private Sub Create(ByVal dr As DataRow, ByVal cYear As Integer, _
            ByVal cMonth As Integer, ByVal RestDayInfo As WorkTimeClassInfo, _
            ByVal PublicHolydaysInfo As WorkTimeClassInfo, ByVal cWorkTime As WorkTime)

            _WorkerID = CIntSafe(dr.Item(0), 0)
            _Worker = CStrSafe(dr.Item(1))
            _WorkerPosition = CStrSafe(dr.Item(2))
            _ContractSerial = CStrSafe(dr.Item(3))
            _ContractNumber = CIntSafe(dr.Item(4), 0)
            _WorkerLoad = CDblSafe(dr.Item(5), 3, 0)
            _DayList = DayWorkTimeList.NewDayWorkTimeList(cYear, cMonth, _WorkerLoad, _
                RestDayInfo, PublicHolydaysInfo)
            _QuotaDays = cWorkTime.WorkDaysFor5WorkDayWeek
            _QuotaHours = cWorkTime.WorkHoursFor5WorkDayWeek

            RecalculateTotals(False)

            ValidationRules.CheckRules()

        End Sub

        Private Sub Fetch(ByVal dr As DataRow, ByVal DayWorkTimeDataTable As DataTable, _
            ByVal cYear As Integer, ByVal cMonth As Integer)

            _ID = CIntSafe(dr.Item(0), 0)
            _WorkerID = CIntSafe(dr.Item(1), 0)
            _Worker = CStrSafe(dr.Item(2)).Trim
            _WorkerPosition = CStrSafe(dr.Item(3)).Trim
            _WorkerLoad = CDblSafe(dr.Item(4), 3, 0)
            _ContractSerial = CStrSafe(dr.Item(5)).Trim
            _ContractNumber = CIntSafe(dr.Item(6), 0)
            _QuotaDays = CIntSafe(dr.Item(7), 0)
            _QuotaHours = CDblSafe(dr.Item(8), ROUNDWORKTIME, 0)
            _TotalDays = CIntSafe(dr.Item(9), 0)
            _TotalHours = CDblSafe(dr.Item(10), ROUNDWORKTIME, 0)

            _DayList = DayWorkTimeList.GetDayWorkTimeList(DayWorkTimeDataTable, Me, cYear, cMonth)

            ValidationRules.CheckRules()

            MarkOld()

        End Sub

        Friend Sub Insert(ByVal parent As WorkTimeSheet)

            Dim myComm As New SQLCommand("InsertWorkTimeItem")
            AddWithParams(myComm)
            myComm.AddParam("?AA", _WorkerID)
            myComm.AddParam("?AB", _ContractSerial.Trim)
            myComm.AddParam("?AC", _ContractNumber)
            myComm.AddParam("?PD", parent.ID)

            myComm.Execute()

            _ID = Convert.ToInt32(myComm.LastInsertID)

            _DayList.Update(Me)

            MarkOld()

        End Sub

        Friend Sub Update(ByVal parent As WorkTimeSheet)

            Dim myComm As New SQLCommand("UpdateWorkTimeItem")
            myComm.AddParam("?CD", _ID)
            AddWithParams(myComm)

            myComm.Execute()

            _DayList.Update(Me)

            MarkOld()

        End Sub

        Friend Sub DeleteSelf()

            Dim myComm As New SQLCommand("DeleteWorkTimeItem")
            myComm.AddParam("?CD", _ID)

            myComm.Execute()

            myComm = New SQLCommand("DeleteDayWorkTimeListForWorkTimeItem")
            myComm.AddParam("?CD", _ID)

            myComm.Execute()

            MarkNew()

        End Sub

        Private Sub AddWithParams(ByRef myComm As SQLCommand)

            myComm.AddParam("?AD", _QuotaDays)
            myComm.AddParam("?AE", CRound(_QuotaHours, ROUNDWORKTIME))
            myComm.AddParam("?AF", _TotalDays)
            myComm.AddParam("?AG", CRound(_TotalHours, ROUNDWORKTIME))

        End Sub

#End Region

    End Class

End Namespace