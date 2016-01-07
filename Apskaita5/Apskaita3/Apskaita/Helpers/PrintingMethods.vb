Imports ApskaitaObjects.ActiveReports
Imports ApskaitaObjects
Imports AccCommon
Module PrintingMethods

    Public Sub PrintObject(ByVal Obj As Object, ByVal ShowPreview As Boolean, _
        Optional ByVal Version As Integer = 1, Optional ByVal mPrinterName As String = "", _
        Optional ByVal DefaultFileName As String = "Report")

        If Obj Is Nothing Then Throw New ArgumentNullException("Klaida. Nerastas spausdintinas objektas.")

        Dim ReportFileName As String = ""
        Dim NumberOfTablesInUse As Integer = 0

        Try

            Dim RD As AccControls.ReportData = MapObjToReport(Obj, ReportFileName, _
                NumberOfTablesInUse, Version)

            Dim ReportFileStream As Byte() = GetFormFromFileStream(Obj, (Version = 1))

            ReportFileName = AccControls.PublicFunctions.AppPath() & "\Reports\" & ReportFileName

            AccControls.PrintReport(ShowPreview, MDIParent1, RD, NumberOfTablesInUse, _
                ReportFileStream, ReportFileName, DefaultFileName, mPrinterName)

        Catch ex As Exception
            ShowError(ex)
        End Try

    End Sub

    Public Sub PrintObjectList(Of T)(ByVal Obj As BusinessObjectCollection(Of T), _
        Optional ByVal Version As Integer = 1)

        If Obj Is Nothing OrElse Obj.Result.Count < 1 Then
            Throw New ArgumentNullException("Klaida. Nerastas spausdintinas objektas.")
        End If

        Dim printerName As String = ""
        Dim numberOfCopies As Short = 1

        Using dlgPrint As New PrintDialog
            If dlgPrint.ShowDialog() <> DialogResult.OK Then Exit Sub
            printerName = dlgPrint.PrinterSettings.PrinterName
            numberOfCopies = dlgPrint.PrinterSettings.Copies
        End Using

        For Each item As T In Obj.Result
            PrintObject(item, False, Version, printerName)
        Next

    End Sub

    Public Sub SendObjectToEmail(ByVal obj As Object, ByVal emailAddress As String, _
        ByVal emailSubject As String, ByVal emailMessageBody As String, _
        Optional ByVal version As Integer = 0, Optional ByVal fileName As String = "")

        If obj Is Nothing Then Throw New NullReferenceException("Klaida. Nerastas spausdintinas objektas.")

        Dim reportFileStream As Byte() = GetFormFromFileStream(obj, (version = 1))

        Dim filePath As String = GetEmailAttachment(obj, version, fileName, reportFileStream)

        SendEmail(emailAddress, emailSubject, emailMessageBody, New String() {filePath})

    End Sub

    Public Sub SendObjectListToEmail(Of T)(ByVal obj As BusinessObjectCollection(Of T), _
        ByVal emailAddress As String, ByVal emailSubject As String, ByVal emailMessageBody As String, _
        Optional ByVal version As Integer = 0)

        If obj Is Nothing OrElse obj.Result.Count < 1 Then _
            Throw New NullReferenceException("Klaida. Nerastas spausdintinas objektas.")

        Dim reportFileStream As Byte() = GetFormFromFileStream(obj, (version = 1))

        Dim filePathList As New List(Of String)

        Dim fileName As String
        Dim counter As Integer = 1
        For Each item As T In obj.Result
            If TypeOf item Is Documents.InvoiceMade Then
                fileName = CType(CType(item, Object), Documents.InvoiceMade).GetFileName
            Else
                fileName = String.Format("{0}({1})", GetType(T).Name, counter.ToString)
            End If
            filePathList.Add(GetEmailAttachment(obj, version, fileName, reportFileStream))
            counter += 1
        Next

        SendEmail(emailAddress, emailSubject, emailMessageBody, filePathList.ToArray)

    End Sub

    Private Function GetEmailAttachment(ByVal obj As Object, ByVal version As Integer, _
        ByVal fileName As String, ByVal reportFileStream As Byte()) As String

        If obj Is Nothing Then Throw New ArgumentNullException("Obj")

        Dim result As String = ""

        Try

            If TypeOf obj Is InvoiceInfo.InvoiceInfo Then

                result = IO.Path.Combine(IO.Path.GetTempPath, fileName & ".xml")

                IO.File.WriteAllText(result, InvoiceInfo.ToXmlString(Of InvoiceInfo.InvoiceInfo) _
                    (DirectCast(obj, InvoiceInfo.InvoiceInfo)))

            Else

                Dim reportFileName As String = ""
                Dim numberOfTablesInUse As Integer = 0

                Dim RD As AccControls.ReportData = MapObjToReport(obj, reportFileName, _
                    numberOfTablesInUse, version)

                reportFileName = AccControls.PublicFunctions.AppPath() & "\Reports\" & reportFileName

                result = AccControls.SaveReportToPDF(RD, numberOfTablesInUse, _
                    reportFileStream, reportFileName, fileName)

            End If

        Catch ex As Exception
            Throw New Exception("Klaida. Nepavyko generuoti ir (ar) išsaugoti dokumento: " & ex.Message, ex)
        End Try

        Return result

    End Function

    Private Sub SendEmail(ByVal emailAddress As String, ByVal emailSubject As String, _
        ByVal emailMessageBody As String, ByVal filePath As String())

        If filePath Is Nothing OrElse filePath.Length < 1 Then
            Throw New ArgumentNullException("filePath")
        End If

        If Not MyCustomSettings.UseDefaultEmailClient Then

            If StringIsNullOrEmpty(emailAddress) Then
                Throw New Exception("Klaida. Nenurodytas e-pašto adresas.")
            End If

            Dim msg As New Net.Mail.MailMessage(MyCustomSettings.UserEmail, emailAddress, _
                emailSubject, emailMessageBody)

            For Each path As String In filePath
                Dim Att As New Net.Mail.Attachment(path)
                msg.Attachments.Add(Att)
            Next

            Dim client As New Net.Mail.SmtpClient(MyCustomSettings.SmtpServer)
            If Not String.IsNullOrEmpty(MyCustomSettings.SmtpPort.Trim) Then
                client.Port = Integer.Parse(MyCustomSettings.SmtpPort.Trim)
            Else
                client.Port = 27
            End If
            client.EnableSsl = MyCustomSettings.UseSslForEmail
            If MyCustomSettings.UseAuthForEmail Then
                client.UseDefaultCredentials = False
                client.Credentials = New Net.NetworkCredential(MyCustomSettings.UserEmailAccount, _
                    MyCustomSettings.UserEmailPassword)
            Else
                client.Credentials = Net.CredentialCache.DefaultNetworkCredentials
            End If
            client.DeliveryMethod = Net.Mail.SmtpDeliveryMethod.Network
            client.Timeout = Integer.MaxValue

            client.Send(msg)

            Try
                For Each path As String In filePath
                    IO.File.Delete(path)
                Next
            Catch ex As Exception
            End Try

        Else

            Dim mapi As New SendFileTo.MAPI
            If Not StringIsNullOrEmpty(emailAddress) Then mapi.AddRecipientTo(emailAddress)
            For Each path As String In filePath
                mapi.AddAttachment(path)
            Next

            If MyCustomSettings.ShowDefaultMailClientWindow Then
                mapi.SendMailPopup(emailSubject, emailMessageBody)
            Else
                mapi.SendMailDirect(emailSubject, emailMessageBody)
            End If

            If Not mapi.GetLastError.Trim.ToLower.StartsWith("ok") Then
                Try
                    For Each path As String In filePath
                        IO.File.Delete(path)
                    Next
                Catch ex As Exception
                End Try
                Throw New Exception("Klaida atidarant e-pašto klientą: " & mapi.GetLastError)
            End If

        End If

    End Sub


    Private Function GetFormFromFileStream(ByVal Obj As Object, _
        ByVal ForceLithuanianRegion As Boolean) As Byte()

        If Obj Is Nothing Then Throw New NullReferenceException( _
            "Klaida. Objektas (dokumentas) negali būti null metode GetFormFromFileStream.")

        If Not TypeOf Obj Is Documents.InvoiceMade Then Return Nothing

        Dim cCompany As HelperLists.CompanyRegionalInfo = GetRegionalData( _
            GetLanguageCodeByObject(Obj, ForceLithuanianRegion))

        If TypeOf Obj Is Documents.InvoiceMade Then
            If Not cCompany.InvoiceForm Is Nothing AndAlso cCompany.InvoiceForm.Length > 50 Then _
                Return cCompany.InvoiceForm

        End If

        Return Nothing

    End Function

    Private Function GetLanguageCodeByObject(ByVal Obj As Object, ByVal ForceLithuanianRegion As Boolean) As String

        Dim LanguageCode As String = LanguageCodeLith.Trim.ToUpper

        If Obj Is Nothing OrElse ForceLithuanianRegion Then Return LanguageCode

        If TypeOf Obj Is Documents.InvoiceMade AndAlso Not DirectCast(Obj, Documents.InvoiceMade).IsDoomyInvoice _
            AndAlso Not DirectCast(Obj, Documents.InvoiceMade).LanguageCode Is Nothing AndAlso _
            Not String.IsNullOrEmpty(DirectCast(Obj, Documents.InvoiceMade).LanguageCode.Trim) AndAlso _
            DirectCast(Obj, Documents.InvoiceMade).LanguageCode.Trim.ToUpper <> LanguageCode Then

            LanguageCode = DirectCast(Obj, Documents.InvoiceMade).LanguageCode.Trim.ToUpper

        End If

        Return LanguageCode

    End Function

    Friend Function GetRegionalData(ByVal LanguageCode As String, _
        Optional ByVal ThrowOnNotFound As Boolean = True) As HelperLists.CompanyRegionalInfo

        If StringIsNullOrEmpty(LanguageCode) OrElse Not IsLanguageCodeValid(LanguageCode) Then
            LanguageCode = LanguageCodeLith.Trim.ToUpper
        End If

        Dim result As HelperLists.CompanyRegionalInfo

        Try

            Using busy As New StatusBusy
                result = HelperLists.CompanyRegionalInfoList.GetList.GetItemByLanguageCode(LanguageCode)
            End Using

        Catch ex As Exception
            Throw New Exception("Klaida. Nepavyko gauti išsamių įmonės duomenų (regioninių): " _
                & vbCrLf & ex.Message, ex)
        End Try

        If result Is Nothing AndAlso ThrowOnNotFound Then Throw New Exception( _
            "Klaida. Nėra įvesta įmonės duomenų kalbai '" & GetLanguageName(LanguageCode, False) & "'.")

        Return result

    End Function


    Friend Function MapObjToReport(ByVal Obj As Object, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer) As AccControls.ReportData

        If Obj Is Nothing Then Throw New NullReferenceException( _
            "Klaida. Objektas (dokumentas) negali būti null metode MapObjToReport.")

        Dim AddSignatureAndLogo As Boolean = (TypeOf Obj Is Documents.InvoiceMade _
            OrElse TypeOf Obj Is Documents.TillIncomeOrder _
            OrElse TypeOf Obj Is Documents.TillSpendingsOrder)

        Dim ForceLithuanianRegion As Boolean = (Version = 1)

        Dim RD As AccControls.ReportData = GetDataSetForReport( _
            GetLanguageCodeByObject(Obj, ForceLithuanianRegion), AddSignatureAndLogo, _
            ForceLithuanianRegion, True)

        If RD Is Nothing Then
            Throw New Exception("Klaida. Nepavyko gauti įmonės duomenų, reikalingų ataskaitos generavimui.")
        End If

        If TypeOf Obj Is General.JournalEntry Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, General.JournalEntry), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is ActiveReports.BookEntryInfoListParent Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, ActiveReports.BookEntryInfoListParent), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is ActiveReports.PersonInfoItemList Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, ActiveReports.PersonInfoItemList), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is General.AccountList Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, General.AccountList), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is ActiveReports.JournalEntryInfoList Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, ActiveReports.JournalEntryInfoList), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Documents.BankOperationItemList Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Documents.BankOperationItemList), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Documents.BankOperation Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Documents.BankOperation), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is ActiveReports.Declaration Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, ActiveReports.Declaration), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is ActiveReports.ImprestSheetInfoList Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, ActiveReports.ImprestSheetInfoList), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Workers.ImprestSheet Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Workers.ImprestSheet), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is ActiveReports.PayOutNaturalPersonInfoList Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, ActiveReports.PayOutNaturalPersonInfoList), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is ActiveReports.WageSheetInfoList Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, ActiveReports.WageSheetInfoList), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is ActiveReports.WorkerWageInfoReport Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, ActiveReports.WorkerWageInfoReport), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Workers.WageSheet Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Workers.WageSheet), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is LongTermAssetInfoList Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, LongTermAssetInfoList), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is ActiveReports.LongTermAssetOperationInfoListParent Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, ActiveReports.LongTermAssetOperationInfoListParent), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is ActiveReports.AdvanceReportInfoList Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, ActiveReports.AdvanceReportInfoList), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is ActiveReports.CashOperationInfoList Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, ActiveReports.CashOperationInfoList), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is ActiveReports.InvoiceInfoItemList Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, ActiveReports.InvoiceInfoItemList), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Documents.AdvanceReport Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Documents.AdvanceReport), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Documents.TillIncomeOrder Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Documents.TillIncomeOrder), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Documents.TillSpendingsOrder Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Documents.TillSpendingsOrder), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is ActiveReports.FinancialStatementsInfo Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, ActiveReports.FinancialStatementsInfo), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is ActiveReports.DebtInfoList Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, ActiveReports.DebtInfoList), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Documents.InvoiceMade Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Documents.InvoiceMade), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is ActiveReports.WorkTimeSheetInfoList Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, ActiveReports.WorkTimeSheetInfoList), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Workers.WorkTimeSheet Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Workers.WorkTimeSheet), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Workers.Contract Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Workers.Contract), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Workers.ContractUpdate Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Workers.ContractUpdate), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Goods.GoodsComplexOperationDiscard Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Goods.GoodsComplexOperationDiscard), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Goods.GoodsComplexOperationInternalTransfer Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Goods.GoodsComplexOperationInternalTransfer), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Goods.GoodsComplexOperationInventorization Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Goods.GoodsComplexOperationInventorization), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Goods.GoodsComplexOperationPriceCut Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Goods.GoodsComplexOperationPriceCut), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Goods.GoodsComplexOperationProduction Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Goods.GoodsComplexOperationProduction), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Goods.GoodsOperationAccountChange Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Goods.GoodsOperationAccountChange), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Goods.GoodsOperationAcquisition Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Goods.GoodsOperationAcquisition), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Goods.GoodsOperationAdditionalCosts Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Goods.GoodsOperationAdditionalCosts), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Goods.GoodsOperationDiscard Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Goods.GoodsOperationDiscard), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Goods.GoodsOperationDiscount Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Goods.GoodsOperationDiscount), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Goods.GoodsOperationPriceCut Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Goods.GoodsOperationPriceCut), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Goods.GoodsOperationTransfer Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Goods.GoodsOperationTransfer), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Goods.ProductionCalculation Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Goods.ProductionCalculation), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is ActiveReports.GoodsOperationInfoListParent Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, ActiveReports.GoodsOperationInfoListParent), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is ActiveReports.GoodsTurnoverInfoList Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, ActiveReports.GoodsTurnoverInfoList), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Assets.OperationAccountChange Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Assets.OperationAccountChange), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Assets.OperationAcquisitionValueIncrease Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Assets.OperationAcquisitionValueIncrease), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Assets.OperationAmortization Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Assets.OperationAmortization), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Assets.OperationAmortizationPeriodChange Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Assets.OperationAmortizationPeriodChange), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Assets.OperationDiscard Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Assets.OperationDiscard), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Assets.OperationOperationalStatusChange Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Assets.OperationOperationalStatusChange), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Assets.OperationTransfer Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Assets.OperationTransfer), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Assets.OperationValueChange Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Assets.OperationValueChange), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Assets.ComplexOperationAmortization Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Assets.ComplexOperationAmortization), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Assets.ComplexOperationDiscard Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Assets.ComplexOperationDiscard), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Assets.ComplexOperationOperationalStatusChange Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Assets.ComplexOperationOperationalStatusChange), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Assets.ComplexOperationValueChange Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Assets.ComplexOperationValueChange), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is ActiveReports.UnsettledPersonInfoList Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, ActiveReports.UnsettledPersonInfoList), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is ActiveReports.WorkersVDUInfo Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, ActiveReports.WorkersVDUInfo), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is ActiveReports.WorkerHolidayInfo Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, ActiveReports.WorkerHolidayInfo), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is WageSheetItem Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, WageSheetItem), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is Workers.HolidayPayReserve Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, Workers.HolidayPayReserve), ReportFileName, NumberOfTablesInUse, Version)
        ElseIf TypeOf Obj Is ActiveReports.ServiceTurnoverInfoList Then
            MapObjectDetailsToReport(RD, DirectCast(Obj, ActiveReports.ServiceTurnoverInfoList), ReportFileName, NumberOfTablesInUse, Version)

            'ElseIf TypeOf Obj Is Documents.Offset Then
            '    MapObjectDetailsToReport(RD, DirectCast(Obj, Documents.Offset), ReportFileName, NumberOfTablesInUse, Version)
        Else
            Throw New NotImplementedException("Klaida. Objektas '" & Obj.GetType.FullName & _
                "' negali būti atvaizduotas ataskaitoje.")
        End If

        Return RD

    End Function

    Friend Function GetDataSetForReport(ByVal LanguageCode As String, ByVal AddSignatureAndLogo As Boolean, _
        ByVal ForceLithuanianRegion As Boolean, ByVal ThrowOnRegionalSettingsNotFound As Boolean) As AccControls.ReportData

        If AddSignatureAndLogo Then
            If Not PrepareCache(Nothing, GetType(AccDataAccessLayer.Security.UserProfile)) Then
                Return Nothing
            End If
        End If

        Dim ARI As HelperLists.CompanyRegionalInfo = GetRegionalData(LanguageCode, _
            ThrowOnRegionalSettingsNotFound)
        Dim LithARI As HelperLists.CompanyRegionalInfo = GetRegionalData(LanguageCodeLith.Trim.ToUpper, True)
        Dim CC As Settings.CompanyInfo = GetCurrentCompany()

        Dim RD As New AccControls.ReportData

        RD.TableCompany.Rows.Add()

        RD.TableCompany.Item(0).Column1 = CC.Name & " (į/k " & _
            GetCurrentCompany.Code & ")"
        RD.TableCompany.Item(0).Column2 = CC.Name
        RD.TableCompany.Item(0).Column3 = CC.Code
        RD.TableCompany.Item(0).Column4 = CC.CodeVat
        RD.TableCompany.Item(0).Column5 = CC.Address
        RD.TableCompany.Item(0).Column6 = CC.CodeSODRA
        RD.TableCompany.Item(0).Column7 = CC.Email
        RD.TableCompany.Item(0).Column8 = CC.HeadPerson
        If Not ARI Is Nothing Then
            RD.TableCompany.Item(0).Column9 = ARI.Address
            RD.TableCompany.Item(0).Column10 = ARI.BankAccount
            RD.TableCompany.Item(0).Column11 = ARI.Bank
            RD.TableCompany.Item(0).Column12 = ARI.BankSWIFT
            RD.TableCompany.Item(0).Column13 = ARI.BankAddress
            RD.TableCompany.Item(0).Column14 = ARI.InvoiceInfoLine
            RD.TableCompany.Item(0).Column15 = ARI.Contacts
            RD.TableCompany.Item(0).Column16 = ARI.DiscountName
            RD.TableCompany.Item(0).Column17 = ARI.HeadTitle
            RD.TableCompany.Item(0).Column18 = ARI.MeasureUnitInvoiceMade
        End If
        RD.TableCompany.Item(0).Column19 = CC.BaseCurrency.Trim.ToUpper
        If MyCustomSettings.SignInvoicesWithCompanySignature Then
            RD.TableCompany.Item(0).Column20 = "1"
        ElseIf MyCustomSettings.SignInvoicesWithRemoteUserSignature Then
            RD.TableCompany.Item(0).Column20 = "2"
        ElseIf MyCustomSettings.SignInvoicesWithLocalUserSignature Then
            RD.TableCompany.Item(0).Column20 = "3"
        Else
            RD.TableCompany.Item(0).Column20 = "0"
        End If
        RD.TableCompany.Item(0).Column21 = MyCustomSettings.UserName
        If AddSignatureAndLogo Then
            RD.TableCompany.Item(0).Column22 = AccDataAccessLayer.Security.UserProfile.GetList.Position
            RD.TableCompany.Item(0).Column23 = GetCurrentIdentity.UserRealName
        Else
            RD.TableCompany.Item(0).Column22 = ""
            RD.TableCompany.Item(0).Column23 = ""
        End If
        RD.TableCompany.Item(0).Column24 = CC.Accountant
        RD.TableCompany.Item(0).Column25 = CC.Cashier

        RD.TableGeneral.Rows.Add()

        If AddSignatureAndLogo Then

            Using busy As New StatusBusy
                Try
                    If Not ARI Is Nothing Then RD.TableGeneral.Item(0).P_Column1 = ARI.LogoImage
                    If MyCustomSettings.SignInvoicesWithLocalUserSignature Then
                        RD.TableGeneral.Item(0).P_Column2 = ImageToByteArray( _
                            ByteArrayToImage(Convert.FromBase64String(MyCustomSettings.UserSignature)))
                    ElseIf MyCustomSettings.SignInvoicesWithCompanySignature AndAlso AddSignatureAndLogo Then
                        RD.TableGeneral.Item(0).P_Column2 = ImageToByteArray( _
                            General.Company.GetCompany.HeadPersonSignature)
                    ElseIf MyCustomSettings.SignInvoicesWithRemoteUserSignature AndAlso AddSignatureAndLogo Then
                        RD.TableGeneral.Item(0).P_Column2 = ImageToByteArray( _
                            AccDataAccessLayer.Security.UserProfile.GetList.Signature)
                    End If
                Catch ex As Exception
                    Throw New Exception("Klaida. Nepavyko gauti įmonės duomenų, " _
                        & "reikalingų ataskaitos generavimui.", ex)
                End Try
            End Using

        End If

        Return RD

    End Function

    Friend Sub UpdateReportDataSetWithRegionalData(ByRef RD As AccControls.ReportData, _
        ByVal RegionalData As General.CompanyRegionalData)

        If RegionalData Is Nothing Then Exit Sub

        RD.TableCompany.Item(0).Column9 = RegionalData.Address
        RD.TableCompany.Item(0).Column10 = RegionalData.BankAccount
        RD.TableCompany.Item(0).Column11 = RegionalData.Bank
        RD.TableCompany.Item(0).Column12 = RegionalData.BankSWIFT
        RD.TableCompany.Item(0).Column13 = RegionalData.BankAddress
        RD.TableCompany.Item(0).Column14 = RegionalData.InvoiceInfoLine
        RD.TableCompany.Item(0).Column15 = RegionalData.Contacts
        RD.TableCompany.Item(0).Column16 = RegionalData.DiscountName
        RD.TableCompany.Item(0).Column17 = RegionalData.HeadTitle
        RD.TableCompany.Item(0).Column18 = RegionalData.MeasureUnitInvoiceMade
        RD.TableGeneral.Item(0).P_Column1 = ImageToByteArray(RegionalData.LogoImage)

    End Sub

#Region "Typed mappers"

#Region "General"

    ''' <summary>
    ''' Map <see cref="General.JournalEntry">JournalEntry</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As General.JournalEntry, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.Date.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.DocNumber
        RD.TableGeneral.Item(0).Column3 = R_Obj.Content
        If R_Obj.Person Is Nothing Then
            RD.TableGeneral.Item(0).Column4 = "Nenustatyta"
        Else
            RD.TableGeneral.Item(0).Column4 = R_Obj.Person.ToString
        End If
        RD.TableGeneral.Item(0).Column5 = R_Obj.DocTypeHumanReadable
        RD.TableGeneral.Item(0).Column7 = DblParser(R_Obj.DebetSum)
        RD.TableGeneral.Item(0).Column8 = DblParser(R_Obj.CreditSum)

        For Each item As General.BookEntry In R_Obj.DebetList
            RD.Table1.Rows.Add()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.Account.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = DblParser(item.Amount)
            If item.Person Is Nothing Then
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = "Nenustatyta"
            Else
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.Person.ToString
            End If
        Next
        For Each item As General.BookEntry In R_Obj.CreditList
            RD.Table2.Rows.Add()
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column1 = item.Account.ToString
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column2 = DblParser(item.Amount)
            If item.Person Is Nothing Then
                RD.Table2.Item(RD.Table2.Rows.Count - 1).Column3 = "Nenustatyta"
            Else
                RD.Table2.Item(RD.Table2.Rows.Count - 1).Column3 = item.Person.ToString
            End If
        Next

        ReportFileName = "R_JournalEntry.rdlc"
        NumberOfTablesInUse = 2

    End Sub

    ''' <summary>
    ''' Map <see cref="ActiveReports.BookEntryInfoListParent">BookEntryInfoListParent</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As ActiveReports.BookEntryInfoListParent, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.Account.ToString
        RD.TableGeneral.Item(0).Column2 = R_Obj.DateFrom.ToShortDateString
        RD.TableGeneral.Item(0).Column3 = R_Obj.DateTo.ToShortDateString
        If R_Obj.IncludeSubAccounts Then
            RD.TableGeneral.Item(0).Column4 = "X"
        Else
            RD.TableGeneral.Item(0).Column4 = ""
        End If
        If R_Obj.Group IsNot Nothing AndAlso R_Obj.Group.ID > 0 Then
            RD.TableGeneral.Item(0).Column5 = R_Obj.Group.Name
            RD.TableGeneral.Item(0).Column6 = "Netaikoma"
        Else
            RD.TableGeneral.Item(0).Column5 = "Netaikoma"
            If R_Obj.Person IsNot Nothing AndAlso R_Obj.Person.ID > 0 Then
                RD.TableGeneral.Item(0).Column6 = R_Obj.Person.ToString
            Else
                RD.TableGeneral.Item(0).Column6 = "Netaikoma"
            End If
        End If
        RD.TableGeneral.Item(0).Column7 = DblParser(R_Obj.DebetBalanceStart)
        RD.TableGeneral.Item(0).Column8 = DblParser(R_Obj.CreditBalanceStart)
        RD.TableGeneral.Item(0).Column9 = DblParser(R_Obj.DebetTurnover)
        RD.TableGeneral.Item(0).Column10 = DblParser(R_Obj.CreditTurnover)
        RD.TableGeneral.Item(0).Column11 = DblParser(R_Obj.DebetBalanceEnd)
        RD.TableGeneral.Item(0).Column12 = DblParser(R_Obj.CreditBalanceEnd)

        Dim accountsInfo As HelperLists.AccountInfoList = Nothing
        Try
            accountsInfo = HelperLists.AccountInfoList.GetList
        Catch ex As Exception
        End Try
        If Not accountsInfo Is Nothing Then
            RD.TableGeneral.Item(0).Column13 = accountsInfo.GetAccountByID(R_Obj.Account).Name
        Else
            RD.TableGeneral.Item(0).Column13 = ""
        End If

        For Each item As ActiveReports.BookEntryInfo In R_Obj.ItemsSortable
            RD.Table1.Rows.Add()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.JournalEntryDate.ToShortDateString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.DocNumber
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.Content
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = DblParser(item.DebetTurnover)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = DblParser(item.CreditTurnover)
            If Not item.PersonID > 0 Then
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = "Nenustatyta"
            Else
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = item.Person
            End If
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = item.BookEntriesString
        Next

        ReportFileName = "R_BookEntryInfoList.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="HelperLists.PersonInfoList">PersonInfoList</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As ActiveReports.PersonInfoItemList, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.LikeString
        RD.TableGeneral.Item(0).Column2 = BooleanToCheckMark(R_Obj.ShowClients)
        RD.TableGeneral.Item(0).Column3 = BooleanToCheckMark(R_Obj.ShowSuppliers)
        RD.TableGeneral.Item(0).Column4 = BooleanToCheckMark(R_Obj.ShowWorkers)

        For Each item As ActiveReports.PersonInfoItem In R_Obj.GetSortedList
            RD.Table1.Rows.Add()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.Name
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.Code
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.Address
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.CodeVAT
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = item.CodeSODRA
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = item.BankAccount
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = item.Bank
            ' RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = item.AssignedToGroups
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = item.AccountAgainstBankBuyer.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = item.AccountAgainstBankSupplyer.ToString
        Next

        ReportFileName = "R_Persons.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="General.AccountList">AccountList</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As General.AccountList, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        For Each item As General.Account In R_Obj
            RD.Table1.Rows.Add()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.ID.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.Name
            If Not item.AssociatedReportItem Is Nothing Then
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.AssociatedReportItem.ToString()
            End If
        Next

        ReportFileName = "R_AccountList.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="ActiveReports.JournalEntryInfoList">JournalEntryInfoList</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As ActiveReports.JournalEntryInfoList, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.DateFrom.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.DateTo.ToShortDateString
        If R_Obj.ApplyDocTypeFilter Then
            RD.TableGeneral.Item(0).Column3 = ConvertEnumHumanReadable(R_Obj.DocTypeFilter)
        Else
            RD.TableGeneral.Item(0).Column3 = "Netaikyta"
        End If
        RD.TableGeneral.Item(0).Column4 = R_Obj.ContentFilter

        If R_Obj.PersonGroupFilter > 0 Then
            RD.TableGeneral.Item(0).Column5 = R_Obj.PersonGroupName
            RD.TableGeneral.Item(0).Column6 = "Netaikyta"
        ElseIf R_Obj.PersonFilter > 0 Then
            RD.TableGeneral.Item(0).Column5 = "Netaikyta"
            RD.TableGeneral.Item(0).Column6 = R_Obj.PersonName
        Else
            RD.TableGeneral.Item(0).Column5 = "Netaikyta"
            RD.TableGeneral.Item(0).Column6 = "Netaikyta"
        End If

        For Each item As ActiveReports.JournalEntryInfo In R_Obj.GetItemsSortable
            RD.Table1.Rows.Add()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.Date.ToShortDateString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.DocNumber
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.Content
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.Person
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = DblParser(item.Ammount)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = item.BookEntries
        Next

        ReportFileName = "R_GeneralLedger.rdlc"
        NumberOfTablesInUse = 1

    End Sub

#End Region

#Region "Documents"

    ''' <summary>
    ''' Map <see cref="Documents.BankOperationItemList">BankOperationItemList</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Documents.BankOperationItemList, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        'RD.TableGeneral.Item(0).Column1 = R_Obj.DateFrom.ToShortDateString
        'RD.TableGeneral.Item(0).Column2 = R_Obj.DateTo.ToShortDateString
        RD.TableGeneral.Item(0).Column3 = R_Obj.SourceType
        RD.TableGeneral.Item(0).Column4 = R_Obj.Account.ToString
        'If R_Obj.CurrentFilterState Then
        '    RD.TableGeneral.Item(0).Column5 = "X"
        'Else
        '    RD.TableGeneral.Item(0).Column5 = ""
        'End If
        'If R_Obj.BalanceStart >= 0 Then
        '    RD.TableGeneral.Item(0).Column6 = D_Parser(R_Obj.BalanceStart)
        '    RD.TableGeneral.Item(0).Column7 = ""
        'Else
        '    RD.TableGeneral.Item(0).Column6 = ""
        '    RD.TableGeneral.Item(0).Column7 = D_Parser(-R_Obj.BalanceStart)
        'End If
        'RD.TableGeneral.Item(0).Column8 = D_Parser(R_Obj.TotalIncome)
        'RD.TableGeneral.Item(0).Column9 = D_Parser(R_Obj.TotalSpendings)
        'If R_Obj.BalanceEnd >= 0 Then
        '    RD.TableGeneral.Item(0).Column10 = D_Parser(R_Obj.BalanceEnd)
        '    RD.TableGeneral.Item(0).Column11 = ""
        'Else
        '    RD.TableGeneral.Item(0).Column10 = ""
        '    RD.TableGeneral.Item(0).Column11 = D_Parser(-R_Obj.BalanceEnd)
        'End If

        Dim SL As Csla.SortedBindingList(Of Documents.BankOperationItem) = R_Obj.GetSortedList()

        For Each item As Documents.BankOperationItem In SL
            RD.Table1.Rows.Add()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.Date.ToShortDateString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.DocumentNumber
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.UniqueCode
            If item.Person IsNot Nothing Then
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.Person.Code
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = item.Person.Name
            Else
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = ""
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = ""
            End If
            If item.Inflow Then
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = DblParser(item.OriginalSum)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = ""
            Else
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = ""
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = DblParser(item.OriginalSum)
            End If
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = item.AccountCorresponding.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = item.Content
        Next

        ReportFileName = "R_BankTransferList.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Documents.BankOperation">BankOperation</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Documents.BankOperation, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.Date.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.DocumentNumber
        If Not R_Obj.Account Is Nothing AndAlso R_Obj.Account.ID > 0 Then
            RD.TableGeneral.Item(0).Column3 = EnumValueAttribute.ConvertLocalizedName(R_Obj.Account.Type)
            RD.TableGeneral.Item(0).Column4 = R_Obj.Account.Account.ToString
            RD.TableGeneral.Item(0).Column5 = R_Obj.Account.Name
            RD.TableGeneral.Item(0).Column6 = R_Obj.Account.CurrencyCode
            RD.TableGeneral.Item(0).Column7 = R_Obj.Account.BankName
            RD.TableGeneral.Item(0).Column8 = R_Obj.Account.BankAccountNumber
        End If
        If Not R_Obj.Person Is Nothing AndAlso R_Obj.Person.ID > 0 Then
            RD.TableGeneral.Item(0).Column9 = R_Obj.Person.Name
            RD.TableGeneral.Item(0).Column10 = R_Obj.Person.Code
        End If
        RD.TableGeneral.Item(0).Column11 = R_Obj.Content
        RD.TableGeneral.Item(0).Column12 = R_Obj.CurrencyCode
        RD.TableGeneral.Item(0).Column13 = DblParser(R_Obj.CurrencyRate, 6)
        RD.TableGeneral.Item(0).Column14 = DblParser(R_Obj.Sum)
        RD.TableGeneral.Item(0).Column15 = R_Obj.AccountCurrencyRateChangeImpact.ToString
        RD.TableGeneral.Item(0).Column16 = DblParser(R_Obj.CurrencyRateChangeImpact)
        RD.TableGeneral.Item(0).Column17 = DblParser(R_Obj.SumLTL)
        RD.TableGeneral.Item(0).Column18 = DblParser(R_Obj.CurrencyRateInAccount, 6)
        RD.TableGeneral.Item(0).Column19 = DblParser(R_Obj.SumInAccount)
        RD.TableGeneral.Item(0).Column20 = R_Obj.UniqueCode
        If R_Obj.IsDebit Then
            RD.TableGeneral.Item(0).Column21 = "Debetas"
        Else
            RD.TableGeneral.Item(0).Column21 = "Kreditas"
        End If

        For Each item As General.BookEntry In R_Obj.BookEntryItemsSorted
            RD.Table1.Rows.Add()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.Account.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = DblParser(item.Amount)
            If Not item.Person Is Nothing AndAlso item.Person.ID > 0 Then
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.Person.Name
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.Person.Code
            End If
        Next

        ReportFileName = "R_BankTransfer.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="ActiveReports.AdvanceReportInfoList">AdvanceReportInfoList</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As ActiveReports.AdvanceReportInfoList, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.DateFrom.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.DateTo.ToShortDateString
        If Not R_Obj.Person Is Nothing AndAlso R_Obj.Person.ID > 0 Then
            RD.TableGeneral.Item(0).Column3 = R_Obj.Person.Name
            RD.TableGeneral.Item(0).Column4 = R_Obj.Person.Code
        End If

        Dim SumIncome As Double = 0
        Dim SumExpenses As Double = 0

        For Each item As ActiveReports.AdvanceReportInfo In R_Obj.GetSortedList

            RD.Table1.Rows.Add()

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.Date.ToShortDateString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.DocumentNumber
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.PersonName
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.PersonCode
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = item.Account.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = item.Content
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = DblParser(item.ExpensesSum)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = DblParser(item.ExpensesSumVat)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = DblParser(item.ExpensesSumTotal)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = DblParser(item.IncomeSum)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = DblParser(item.IncomeSumVat)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(item.IncomeSumTotal)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = item.CurrencyCode
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(item.CurrencyRate, 6)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = DblParser(item.ExpensesSumLTL)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = DblParser(item.ExpensesSumVatLTL)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = DblParser(item.ExpensesSumTotalLTL)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = DblParser(item.IncomeSumLTL)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = DblParser(item.IncomeSumVatLTL)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column20 = DblParser(item.IncomeSumTotalLTL)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column21 = item.Comments
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column22 = item.CommentsInternal
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column23 = item.TillOrderData

            SumIncome += item.IncomeSumTotalLTL
            SumExpenses += item.ExpensesSumTotalLTL

        Next

        RD.TableGeneral.Item(0).Column5 = DblParser(SumExpenses)
        RD.TableGeneral.Item(0).Column6 = DblParser(SumIncome)

        ReportFileName = "R_AdvanceReportList.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="ActiveReports.CashOperationInfoList">CashOperationInfoList</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As ActiveReports.CashOperationInfoList, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.DateFrom.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.DateTo.ToShortDateString
        If Not R_Obj.Person Is Nothing AndAlso R_Obj.Person.ID > 0 Then
            RD.TableGeneral.Item(0).Column3 = R_Obj.Person.Name
            RD.TableGeneral.Item(0).Column4 = R_Obj.Person.Code
        End If
        If Not R_Obj.Account Is Nothing AndAlso R_Obj.Account.ID > 0 Then
            RD.TableGeneral.Item(0).Column5 = EnumValueAttribute.ConvertLocalizedName(R_Obj.Account.Type)
            RD.TableGeneral.Item(0).Column6 = R_Obj.Account.Account.ToString
            RD.TableGeneral.Item(0).Column7 = R_Obj.Account.Name
            RD.TableGeneral.Item(0).Column8 = R_Obj.Account.CurrencyCode
            RD.TableGeneral.Item(0).Column9 = R_Obj.Account.BankName
            RD.TableGeneral.Item(0).Column10 = R_Obj.Account.BankAccountNumber
        End If

        RD.TableGeneral.Item(0).Column11 = DblParser(R_Obj.BalanceStart)
        RD.TableGeneral.Item(0).Column12 = DblParser(R_Obj.BalanceBookEntryStart)
        RD.TableGeneral.Item(0).Column13 = DblParser(R_Obj.BalanceLTLStart)
        RD.TableGeneral.Item(0).Column14 = DblParser(R_Obj.TurnoverDebit)
        RD.TableGeneral.Item(0).Column15 = DblParser(R_Obj.TurnoverCredit)
        RD.TableGeneral.Item(0).Column16 = DblParser(R_Obj.TurnoverBookEntryDebit)
        RD.TableGeneral.Item(0).Column17 = DblParser(R_Obj.TurnoverBookEntryCredit)
        RD.TableGeneral.Item(0).Column18 = DblParser(R_Obj.TurnoverLTLDebit)
        RD.TableGeneral.Item(0).Column19 = DblParser(R_Obj.TurnoverLTLCredit)
        RD.TableGeneral.Item(0).Column20 = DblParser(R_Obj.TurnOverInListLTLDebit)
        RD.TableGeneral.Item(0).Column21 = DblParser(R_Obj.TurnoverInListLTLCredit)
        RD.TableGeneral.Item(0).Column22 = DblParser(R_Obj.BalanceEnd)
        RD.TableGeneral.Item(0).Column23 = DblParser(R_Obj.BalanceBookEntryEnd)
        RD.TableGeneral.Item(0).Column24 = DblParser(R_Obj.BalanceLTLEnd)

        Dim SumInflow As Double = 0
        Dim SumOutflow As Double = 0
        Dim SumInflowLTL As Double = 0
        Dim SumOutflowLTL As Double = 0
        Dim SumInflowBookEntries As Double = 0
        Dim SumOutflowBookEntries As Double = 0

        For Each item As ActiveReports.CashOperationInfo In R_Obj.GetSortedList

            If Version <> 1 OrElse item.OperationType = DocumentType.TillIncomeOrder _
                OrElse item.OperationType = DocumentType.TillSpendingOrder Then

                RD.Table1.Rows.Add()

                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.Date.ToShortDateString
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.DocumentNumber
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.OperationTypeHumanReadable
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.Person
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = item.Content
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = item.AccountName
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = DblParser(item.Sum)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = item.CurrencyCode
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = DblParser(item.CurrencyRate, 6)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = DblParser(item.SumLTL)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = DblParser(item.SumBookEntry)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(item.CurrencyRateInAccount, 6)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = DblParser(item.SumInAccount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(item.CurrencyRateChangeImpact)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = item.UniqueCode
                If item.Sum > 0 Then
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = "1"
                    SumInflow += item.SumInAccount
                    SumInflowLTL += item.SumLTL
                    SumInflowBookEntries += item.SumLTL
                Else
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = "0"
                    SumOutflow += -item.SumInAccount
                    SumOutflowLTL += -item.SumLTL
                    SumOutflowBookEntries += -item.SumBookEntry
                End If

            End If

        Next

        RD.TableGeneral.Item(0).Column25 = DblParser(SumInflow)
        RD.TableGeneral.Item(0).Column26 = DblParser(SumOutflow)
        RD.TableGeneral.Item(0).Column27 = DblParser(SumInflowLTL)
        RD.TableGeneral.Item(0).Column28 = DblParser(SumOutflowLTL)
        RD.TableGeneral.Item(0).Column29 = DblParser(SumInflowBookEntries)
        RD.TableGeneral.Item(0).Column30 = DblParser(SumOutflowBookEntries)
        RD.TableGeneral.Item(0).Column31 = DblParser(SumInflow - SumOutflow)
        RD.TableGeneral.Item(0).Column32 = DblParser(SumInflowLTL - SumOutflowLTL)
        RD.TableGeneral.Item(0).Column33 = DblParser(SumInflowBookEntries - SumOutflowBookEntries)

        If Version = 0 Then
            ReportFileName = "R_CashOperationInfoList.rdlc"
        ElseIf Version = 1 Then
            ReportFileName = "R_TillBook.rdlc"
        Else
            Throw New NotImplementedException("Klaida. Lėšų apyvartos žiniaraščio " & _
                "spausdinamos formos 3 versijos dar nenupiešė barsukas.")
        End If
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="ActiveReports.InvoiceInfoItemList">InvoiceInfoItemList</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As ActiveReports.InvoiceInfoItemList, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.DateFrom.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.DateTo.ToShortDateString
        RD.TableGeneral.Item(0).Column3 = R_Obj.InfoTypeHumanReadable
        If Not R_Obj.Person Is Nothing AndAlso R_Obj.Person.ID > 0 Then
            RD.TableGeneral.Item(0).Column4 = R_Obj.Person.Name
            RD.TableGeneral.Item(0).Column5 = R_Obj.Person.Code
        End If

        Dim SumLTL As Double = 0
        Dim SumVatLTL As Double = 0
        Dim SumDiscountLTL As Double = 0
        Dim SumDiscountVatLTL As Double = 0
        Dim SumTotalLTL As Double = 0

        For Each item As ActiveReports.InvoiceInfoItem In R_Obj.GetSortedList

            RD.Table1.Rows.Add()

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.Date.ToShortDateString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.Number
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.PersonName
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.PersonCode
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = item.PersonVatCode
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = item.PersonAccount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = item.PersonEmail
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = item.Content
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = item.LanguageName
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = item.CommentsInternal
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = DblParser(item.Sum)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(item.SumVat)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = DblParser(item.SumDiscount)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(item.SumVatDiscount)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = DblParser(item.TotalSumDiscount)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = DblParser(item.TotalSum)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = item.CurrencyCode
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = DblParser(item.CurrencyRate, 6)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = DblParser(item.SumLTL)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column20 = DblParser(item.SumVatLTL)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column21 = DblParser(item.SumDiscountLTL)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column22 = DblParser(item.SumVatDiscountLTL)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column23 = DblParser(item.TotalSumDiscountLTL)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column24 = DblParser(item.TotalSumLTL)

            SumLTL += item.SumLTL
            SumVatLTL += item.SumVatLTL
            SumTotalLTL += item.TotalSumLTL
            SumDiscountLTL += item.SumDiscountLTL
            SumDiscountVatLTL += item.SumVatDiscountLTL

        Next

        RD.TableGeneral.Item(0).Column6 = DblParser(SumLTL)
        RD.TableGeneral.Item(0).Column7 = DblParser(SumVatLTL)
        RD.TableGeneral.Item(0).Column8 = DblParser(SumDiscountLTL)
        RD.TableGeneral.Item(0).Column9 = DblParser(SumDiscountVatLTL)
        RD.TableGeneral.Item(0).Column10 = DblParser(SumTotalLTL)

        ReportFileName = "R_InvoiceInfoList.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Documents.AdvanceReport">AdvanceReport</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Documents.AdvanceReport, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.Date.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.DocumentNumber
        If Not R_Obj.Person Is Nothing AndAlso R_Obj.Person.ID > 0 Then
            RD.TableGeneral.Item(0).Column3 = R_Obj.Person.Name
            RD.TableGeneral.Item(0).Column4 = R_Obj.Person.Code
        End If
        RD.TableGeneral.Item(0).Column5 = R_Obj.Content
        RD.TableGeneral.Item(0).Column6 = R_Obj.Account.ToString
        RD.TableGeneral.Item(0).Column7 = R_Obj.CurrencyCode
        RD.TableGeneral.Item(0).Column8 = DblParser(R_Obj.CurrencyRate, 6)
        RD.TableGeneral.Item(0).Column9 = DblParser(R_Obj.Sum)
        RD.TableGeneral.Item(0).Column10 = DblParser(R_Obj.SumVat)
        RD.TableGeneral.Item(0).Column11 = DblParser(R_Obj.SumTotal)
        RD.TableGeneral.Item(0).Column12 = DblParser(R_Obj.SumLTL)
        RD.TableGeneral.Item(0).Column13 = DblParser(R_Obj.SumVatLTL)
        RD.TableGeneral.Item(0).Column14 = DblParser(R_Obj.SumTotalLTL)
        RD.TableGeneral.Item(0).Column15 = R_Obj.Comments
        RD.TableGeneral.Item(0).Column16 = R_Obj.CommentsInternal
        RD.TableGeneral.Item(0).Column17 = Convert.ToInt32(Math.Floor(R_Obj.SumTotal)).ToString
        RD.TableGeneral.Item(0).Column18 = Convert.ToInt32(CRound((R_Obj.SumTotal _
            - Math.Floor(R_Obj.SumTotal)) * 100, 0)).ToString.PadLeft(2, "0"c)
        RD.TableGeneral.Item(0).Column19 = ConvertDateToWordsLT(R_Obj.Date)
        RD.TableGeneral.Item(0).Column20 = R_Obj.ReportItemsSorted.Count.ToString

        Dim sign As Integer

        For Each item As Documents.AdvanceReportItem In R_Obj.ReportItemsSorted

            RD.Table1.Rows.Add()

            If item.Expenses Then
                sign = 1
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = "1"
            Else
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = "0"
                sign = -1
            End If
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.Date.ToShortDateString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.DocumentNumber
            If Not item.Person Is Nothing AndAlso item.Person.ID > 0 Then
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.Person.Name
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = item.Person.Code
            End If
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = item.Content
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = item.Account.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = item.AccountVat.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = item.AccountCurrencyRateChangeEffect.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = DblParser(item.VatRate)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = DblParser(item.Sum * sign)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(item.SumVat * sign)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = DblParser(item.SumTotal * sign)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(item.SumLTL * sign)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = DblParser(item.SumVatLTL * sign)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = DblParser(item.SumTotalLTL * sign)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = DblParser(item.CurrencyRateChangeEffect)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = item.InvoiceDateAndNumber
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = item.InvoiceContent
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column20 = item.InvoiceCurrencyCode
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column21 = DblParser(item.InvoiceCurrencyRate, 6)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column22 = DblParser(item.InvoiceSumOriginal)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column23 = DblParser(item.InvoiceSumVatOriginal)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column24 = DblParser(item.InvoiceSumTotalOriginal)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column25 = DblParser(item.InvoiceSumLTL)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column26 = DblParser(item.InvoiceSumVatLTL)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column27 = DblParser(item.InvoiceSumTotalLTL)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column28 = DblParser(item.InvoiceSumTotal)

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column29 = Convert.ToInt32(Math.Floor(item.SumTotal)).ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column30 = Convert.ToInt32(CRound((item.SumTotal _
                - Math.Floor(item.SumTotal)) * 100, 0)).ToString.PadLeft(2, "0"c)


        Next

        For Each item As General.BookEntry In R_Obj.GetBookEntryList(BookEntryType.Debetas)

            RD.Table2.Rows.Add()

            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column1 = item.Account.ToString
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column2 = DblParser(item.Amount)
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column3 = Convert.ToInt32(Math.Floor(item.Amount)).ToString
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column4 = Convert.ToInt32(CRound((item.Amount _
                - Math.Floor(item.Amount)) * 100, 0)).ToString.PadLeft(2, "0"c)

        Next

        For Each item As General.BookEntry In R_Obj.GetBookEntryList(BookEntryType.Kreditas)

            RD.Table3.Rows.Add()

            RD.Table3.Item(RD.Table3.Rows.Count - 1).Column1 = item.Account.ToString
            RD.Table3.Item(RD.Table3.Rows.Count - 1).Column2 = DblParser(item.Amount)
            RD.Table3.Item(RD.Table3.Rows.Count - 1).Column3 = Convert.ToInt32(Math.Floor(item.Amount)).ToString
            RD.Table3.Item(RD.Table3.Rows.Count - 1).Column4 = Convert.ToInt32(CRound((item.Amount _
                - Math.Floor(item.Amount)) * 100, 0)).ToString.PadLeft(2, "0"c)

        Next

        ReportFileName = "R_AdvanceReport.rdlc"
        NumberOfTablesInUse = 3

    End Sub

    ''' <summary>
    ''' Map <see cref="Documents.TillIncomeOrder">TillIncomeOrder</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Documents.TillIncomeOrder, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.Date.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = ConvertDateToWordsLT(R_Obj.Date)
        RD.TableGeneral.Item(0).Column3 = R_Obj.DocumentSerial
        RD.TableGeneral.Item(0).Column4 = R_Obj.DocumentNumber.ToString
        RD.TableGeneral.Item(0).Column5 = R_Obj.FullDocumentNumber
        RD.TableGeneral.Item(0).Column6 = R_Obj.Account.Account.ToString
        RD.TableGeneral.Item(0).Column7 = R_Obj.Account.CurrencyCode.Trim.ToUpper
        RD.TableGeneral.Item(0).Column8 = DblParser(R_Obj.CurrencyRateInAccount, 6)
        RD.TableGeneral.Item(0).Column9 = R_Obj.Account.Name
        RD.TableGeneral.Item(0).Column10 = R_Obj.Content
        If Not R_Obj.Payer Is Nothing AndAlso R_Obj.Payer.ID > 0 Then
            RD.TableGeneral.Item(0).Column11 = R_Obj.Payer.Name
            RD.TableGeneral.Item(0).Column12 = R_Obj.Payer.Code
        End If
        RD.TableGeneral.Item(0).Column13 = R_Obj.PayersRepresentative
        RD.TableGeneral.Item(0).Column14 = R_Obj.AttachmentsDescription
        RD.TableGeneral.Item(0).Column15 = R_Obj.AdvanceReportDescription
        RD.TableGeneral.Item(0).Column16 = BooleanToCheckMark(R_Obj.IsUnderPayRoll)
        RD.TableGeneral.Item(0).Column17 = R_Obj.AdditionalContent
        RD.TableGeneral.Item(0).Column18 = DblParser(R_Obj.Sum)
        RD.TableGeneral.Item(0).Column19 = DblParser(R_Obj.SumLTL)
        RD.TableGeneral.Item(0).Column20 = DblParser(R_Obj.SumCorespondences)
        RD.TableGeneral.Item(0).Column21 = DblParser(R_Obj.CurrencyRateChangeImpact)
        RD.TableGeneral.Item(0).Column22 = R_Obj.AccountCurrencyRateChangeImpact.ToString
        RD.TableGeneral.Item(0).Column23 = SumLT(R_Obj.Sum, 0, True, _
            GetCurrencySafe(R_Obj.AccountCurrency, GetCurrentCompany.BaseCurrency))
        RD.TableGeneral.Item(0).Column24 = Convert.ToInt32(Math.Floor(R_Obj.Sum)).ToString
        RD.TableGeneral.Item(0).Column25 = Convert.ToInt32(CRound(CRound(R_Obj.Sum _
            - Math.Floor(R_Obj.Sum)) * 100, 0)).ToString.PadLeft(2, "0"c)

        If Not R_Obj.Payer Is Nothing AndAlso R_Obj.Payer.ID > 0 Then
            Dim PayerStringArray As String() = SplitStringByMaxLength( _
                R_Obj.Payer.Name & " (" & R_Obj.Payer.Code & ") " _
                & R_Obj.PayersRepresentative, 95)
            RD.TableGeneral.Item(0).Column27 = PayerStringArray(0)
            If PayerStringArray.Length > 1 Then _
                RD.TableGeneral.Item(0).Column28 = PayerStringArray(1)
        End If
        Dim ContentStringArray As String() = SplitStringByMaxLength( _
            R_Obj.Content.Trim & " " & R_Obj.AdditionalContent.Trim, 95)
        RD.TableGeneral.Item(0).Column29 = ContentStringArray(0)
        If ContentStringArray.Length > 1 Then _
            RD.TableGeneral.Item(0).Column30 = ContentStringArray(1)
        Dim SumStringArray As String() = SplitStringByMaxLength(SumLT(Math.Floor(R_Obj.Sum), _
            0, False, GetCurrencySafe(R_Obj.AccountCurrency, GetCurrentCompany.BaseCurrency)), 95)
        RD.TableGeneral.Item(0).Column31 = SumStringArray(0)
        If SumStringArray.Length > 1 Then _
            RD.TableGeneral.Item(0).Column32 = SumStringArray(1)
        Dim AttachmentsStringArray As String() = SplitStringByMaxLength( _
            R_Obj.AttachmentsDescription.Trim, 95)
        RD.TableGeneral.Item(0).Column33 = AttachmentsStringArray(0)
        If AttachmentsStringArray.Length > 1 Then _
            RD.TableGeneral.Item(0).Column34 = AttachmentsStringArray(1)

        Dim AccountList As String = ""

        For Each item As General.BookEntry In R_Obj.BookEntryItemsSorted

            RD.Table1.Rows.Add()

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.Account.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = DblParser(item.Amount)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = Convert.ToInt32(Math.Floor(item.Amount)).ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = Convert.ToInt32(CRound((item.Amount _
                - Math.Floor(item.Amount)) * 100, 0)).ToString

            If String.IsNullOrEmpty(AccountList.Trim) Then
                AccountList = item.Account.ToString
            Else
                AccountList = AccountList & ", " & item.Account.ToString
            End If

        Next

        RD.TableGeneral.Item(0).Column26 = AccountList

        ReportFileName = "R_TillOrderIncome.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Documents.TillSpendingsOrder">TillSpendingsOrder</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Documents.TillSpendingsOrder, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.Date.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = ConvertDateToWordsLT(R_Obj.Date)
        RD.TableGeneral.Item(0).Column3 = R_Obj.DocumentSerial
        RD.TableGeneral.Item(0).Column4 = R_Obj.DocumentNumber.ToString
        RD.TableGeneral.Item(0).Column5 = R_Obj.FullDocumentNumber
        RD.TableGeneral.Item(0).Column6 = R_Obj.Account.Account.ToString
        RD.TableGeneral.Item(0).Column7 = R_Obj.Account.CurrencyCode.Trim.ToUpper
        RD.TableGeneral.Item(0).Column8 = DblParser(R_Obj.CurrencyRateInAccount, 6)
        RD.TableGeneral.Item(0).Column9 = R_Obj.Account.Name
        RD.TableGeneral.Item(0).Column10 = R_Obj.Content
        If Not R_Obj.Receiver Is Nothing AndAlso R_Obj.Receiver.ID > 0 Then
            RD.TableGeneral.Item(0).Column11 = R_Obj.Receiver.Name
            RD.TableGeneral.Item(0).Column12 = R_Obj.Receiver.Code
        Else
            RD.TableGeneral.Item(0).Column11 = "Pagal žiniaraštį"
            RD.TableGeneral.Item(0).Column12 = ""
        End If
        RD.TableGeneral.Item(0).Column13 = R_Obj.ReceiversRepresentative
        RD.TableGeneral.Item(0).Column14 = R_Obj.AttachmentsDescription
        RD.TableGeneral.Item(0).Column15 = R_Obj.AdvanceReportDescription
        RD.TableGeneral.Item(0).Column16 = BooleanToCheckMark(R_Obj.IsUnderPayRoll)
        RD.TableGeneral.Item(0).Column17 = R_Obj.AdditionalContent
        RD.TableGeneral.Item(0).Column18 = DblParser(R_Obj.Sum)
        RD.TableGeneral.Item(0).Column19 = DblParser(R_Obj.SumLTL)
        RD.TableGeneral.Item(0).Column20 = DblParser(R_Obj.SumCorespondences)
        RD.TableGeneral.Item(0).Column21 = DblParser(R_Obj.CurrencyRateChangeImpact)
        RD.TableGeneral.Item(0).Column22 = R_Obj.AccountCurrencyRateChangeImpact.ToString
        RD.TableGeneral.Item(0).Column23 = SumLT(CRound(R_Obj.Sum), 0, True, _
            GetCurrencySafe(R_Obj.AccountCurrency, GetCurrentCompany.BaseCurrency))
        RD.TableGeneral.Item(0).Column24 = Convert.ToInt32(Math.Floor(R_Obj.Sum)).ToString
        RD.TableGeneral.Item(0).Column25 = Convert.ToInt32(CRound(CRound(R_Obj.Sum _
            - Math.Floor(R_Obj.Sum)) * 100, 0)).ToString

        If Not R_Obj.Receiver Is Nothing AndAlso R_Obj.Receiver.ID > 0 Then
            Dim PayerStringArray As String() = SplitStringByMaxLength( _
                R_Obj.Receiver.Name & " (" & R_Obj.Receiver.Code & ") " _
                & R_Obj.ReceiversRepresentative, 95)
            RD.TableGeneral.Item(0).Column27 = PayerStringArray(0)
            If PayerStringArray.Length > 1 Then _
                RD.TableGeneral.Item(0).Column28 = PayerStringArray(1)
        Else
            RD.TableGeneral.Item(0).Column27 = "Pagal žiniaraštį"
            RD.TableGeneral.Item(0).Column28 = ""
        End If
        Dim ContentStringArray As String() = SplitStringByMaxLength( _
            R_Obj.Content.Trim & " " & R_Obj.AdditionalContent.Trim, 95)
        RD.TableGeneral.Item(0).Column29 = ContentStringArray(0)
        If ContentStringArray.Length > 1 Then _
            RD.TableGeneral.Item(0).Column30 = ContentStringArray(1)
        Dim SumStringArray As String() = SplitStringByMaxLength(SumLT(Math.Floor(R_Obj.Sum), _
            0, False, GetCurrencySafe(R_Obj.AccountCurrency, GetCurrentCompany.BaseCurrency)), 95)
        RD.TableGeneral.Item(0).Column31 = SumStringArray(0)
        If SumStringArray.Length > 1 Then _
            RD.TableGeneral.Item(0).Column32 = SumStringArray(1)
        Dim AttachmentsStringArray As String() = SplitStringByMaxLength( _
            R_Obj.AttachmentsDescription.Trim, 95)
        RD.TableGeneral.Item(0).Column33 = AttachmentsStringArray(0)
        If AttachmentsStringArray.Length > 1 Then _
            RD.TableGeneral.Item(0).Column34 = AttachmentsStringArray(1)

        Dim AccountList As String = ""

        For Each item As General.BookEntry In R_Obj.BookEntryItemsSorted

            RD.Table1.Rows.Add()

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.Account.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = DblParser(item.Amount)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = Convert.ToInt32(Math.Floor(item.Amount)).ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = Convert.ToInt32(CRound((item.Amount _
                - Math.Floor(item.Amount)) * 100, 0)).ToString

            If String.IsNullOrEmpty(AccountList.Trim) Then
                AccountList = item.Account.ToString
            Else
                AccountList = AccountList & ", " & item.Account.ToString
            End If

        Next

        RD.TableGeneral.Item(0).Column26 = AccountList

        ReportFileName = "R_TillOrderSpendings.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Documents.InvoiceMade">InvoiceMade</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Documents.InvoiceMade, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.Date.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.Serial.Trim
        RD.TableGeneral.Item(0).Column3 = R_Obj.FullNumber.Trim
        RD.TableGeneral.Item(0).Column4 = DblParser(R_Obj.Sum + R_Obj.SumDiscount)
        RD.TableGeneral.Item(0).Column5 = DblParser(R_Obj.SumVat + R_Obj.SumDiscountVat)
        If R_Obj.SumDiscount > 0 Then
            RD.TableGeneral.Item(0).Column6 = DblParser(-R_Obj.SumDiscount)
            RD.TableGeneral.Item(0).Column7 = DblParser(-R_Obj.SumDiscountVat)
        Else
            RD.TableGeneral.Item(0).Column6 = ""
            RD.TableGeneral.Item(0).Column7 = ""
        End If
        RD.TableGeneral.Item(0).Column8 = DblParser(R_Obj.SumTotal)
        RD.TableGeneral.Item(0).Column9 = R_Obj.CurrencyCode
        RD.TableGeneral.Item(0).Column10 = DblParser(R_Obj.CurrencyRate, 6)
        If Not R_Obj.Payer Is Nothing AndAlso R_Obj.Payer.ID > 0 Then
            RD.TableGeneral.Item(0).Column11 = R_Obj.Payer.Name
            RD.TableGeneral.Item(0).Column12 = R_Obj.Payer.Code
            RD.TableGeneral.Item(0).Column13 = R_Obj.Payer.CodeVAT
            RD.TableGeneral.Item(0).Column14 = R_Obj.Payer.Address
        Else
            RD.TableGeneral.Item(0).Column11 = ""
            RD.TableGeneral.Item(0).Column12 = ""
            RD.TableGeneral.Item(0).Column13 = ""
            RD.TableGeneral.Item(0).Column14 = ""
        End If
        RD.TableGeneral.Item(0).Column15 = R_Obj.CustomInfo
        RD.TableGeneral.Item(0).Column16 = R_Obj.CustomInfoAltLng
        RD.TableGeneral.Item(0).Column17 = R_Obj.VatExemptInfo
        RD.TableGeneral.Item(0).Column18 = R_Obj.VatExemptInfoAltLng
        If R_Obj.Type = Documents.InvoiceType.Credit Then
            RD.TableGeneral.Item(0).Column19 = "1"
        ElseIf R_Obj.Type = Documents.InvoiceType.Debit Then
            RD.TableGeneral.Item(0).Column19 = "2"
        Else
            RD.TableGeneral.Item(0).Column19 = "0"
        End If
        If R_Obj.Date.Date >= New Date(2014, 7, 1).Date AndAlso R_Obj.Date.Date <= New Date(2015, 6, 30).Date Then
            If CurrenciesEquals(R_Obj.CurrencyCode, "LTL", GetCurrentCompany.BaseCurrency) Then
                RD.TableGeneral.Item(0).Column20 = DblParser(R_Obj.SumTotal / 3.4528)
                RD.TableGeneral.Item(0).Column21 = "1"
            ElseIf CurrenciesEquals(R_Obj.CurrencyCode, "EUR", GetCurrentCompany.BaseCurrency) Then
                RD.TableGeneral.Item(0).Column20 = DblParser(R_Obj.SumTotal * 3.4528)
                RD.TableGeneral.Item(0).Column21 = "2"
            Else
                RD.TableGeneral.Item(0).Column20 = "0"
                RD.TableGeneral.Item(0).Column21 = "0"
            End If
        Else
            RD.TableGeneral.Item(0).Column20 = ""
            RD.TableGeneral.Item(0).Column21 = "0"
        End If

        RD.TableGeneral.Item(0).Column22 = SumLT(R_Obj.SumTotal, 0, True, R_Obj.CurrencyCode)

        For Each item As Documents.InvoiceMadeItem In R_Obj.InvoiceItemsSorted

            RD.Table1.Rows.Add()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.NameInvoice.Trim
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.NameInvoiceAltLng.Trim
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.MeasureUnit
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = DblParser(item.Ammount, ROUNDAMOUNTINVOICEMADE)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = DblParser(item.UnitValue, ROUNDUNITINVOICEMADE)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = DblParser(item.Sum, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = DblParser(item.VatRate, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = DblParser(item.SumVat, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = DblParser(item.Discount, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = DblParser(item.DiscountVat, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = DblParser(item.SumTotal, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = item.MeasureUnitAltLng
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = DblParser(item.UnitValueLTL, ROUNDUNITINVOICEMADE)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(item.SumLTL, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = DblParser(item.SumVatLTL, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = DblParser(item.DiscountLTL, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = DblParser(item.DiscountVatLTL, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = DblParser(item.SumTotalLTL, 2)

        Next

        If Version > 0 OrElse R_Obj.LanguageCode Is Nothing OrElse String.IsNullOrEmpty(R_Obj.LanguageCode.Trim) _
            OrElse R_Obj.LanguageCode.Trim.ToUpper = LanguageCodeLith.Trim.ToUpper Then
            ReportFileName = "R_Invoice.rdlc"
        Else
            ReportFileName = "R_InvoiceAltLng.rdlc"
        End If

        NumberOfTablesInUse = 1

    End Sub

#End Region

#Region "Assets"

    ''' <summary>
    ''' Map <see cref="LongTermAssetInfoList">LongTermAssetInfoList</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As LongTermAssetInfoList, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.CustomAssetGroup
        RD.TableGeneral.Item(0).Column2 = R_Obj.DateFrom.ToShortDateString
        RD.TableGeneral.Item(0).Column3 = R_Obj.DateTo.ToShortDateString

        For Each item As LongTermAssetInfo In R_Obj.GetSortedList
            RD.Table1.Rows.Add()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.Name
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.MeasureUnit
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.LegalGroup
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.CustomGroup
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = item.AcquisitionDate.ToShortDateString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = item.AcquisitionJournalEntryDocNumber
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = item.AcquisitionJournalEntryDocContent
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = item.InventoryNumber
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = item.AccountAcquisition.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = item.AccountAccumulatedAmortization.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = item.AccountValueIncrease.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = item.AccountValueDecrease.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = item.AccountRevaluedPortionAmmortization.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(item.LiquidationUnitValue)
            If item.ContinuedUsage Then
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = "Taip"
            Else
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = "Ne"
            End If
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = item.DefaultAmortizationPeriod.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = DblParser(item.AcquisitionAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = DblParser(item.AcquisitionAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = DblParser(item.AmortizationAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column20 = DblParser(item.AmortizationAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column21 = DblParser(item.ValueDecreaseAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column22 = DblParser(item.ValueDecreaseAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column23 = DblParser(item.ValueIncreaseAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column24 = DblParser(item.ValueIncreaseAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column25 = DblParser(item.ValueIncreaseAmmortizationAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column26 = DblParser(item.ValueIncreaseAmmortizationAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column27 = DblParser(item.Value)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column28 = DblParser(item.ValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column29 = item.Ammount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column30 = DblParser(item.ValueRevaluedPortion)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column31 = DblParser(item.ValueRevaluedPortionPerUnit, 4)

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column32 = DblParser(item.BeforeAcquisitionAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column33 = DblParser(item.BeforeAcquisitionAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column34 = DblParser(item.BeforeAmortizationAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column35 = DblParser(item.BeforeAmortizationAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column36 = DblParser(item.BeforeValueDecreaseAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column37 = DblParser(item.BeforeValueDecreaseAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column38 = DblParser(item.BeforeValueIncreaseAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column39 = DblParser(item.BeforeValueIncreaseAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column40 = DblParser(item.BeforeValueIncreaseAmmortizationAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column41 = DblParser(item.BeforeValueIncreaseAmmortizationAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column42 = DblParser(item.BeforeValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column43 = DblParser(item.BeforeValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column44 = item.BeforeAmmount.ToString

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column45 = DblParser(item.ChangeAcquisitionAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column46 = DblParser(item.ChangeAcquisitionAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column47 = DblParser(item.ChangeAmortizationAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column48 = DblParser(item.ChangeAmortizationAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column49 = DblParser(item.ChangeValueDecreaseAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column50 = DblParser(item.ChangeValueDecreaseAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column51 = DblParser(item.ChangeValueIncreaseAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column52 = DblParser(item.ChangeValueIncreaseAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column53 = DblParser(item.ChangeValueIncreaseAmmortizationAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column54 = DblParser(item.ChangeValueIncreaseAmmortizationAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column55 = DblParser(item.ChangeValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column56 = DblParser(item.ChangeValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column57 = item.ChangeAmmount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column58 = item.ChangeAmmountAcquired.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column59 = DblParser(item.ChangeValueAcquired)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column60 = item.ChangeAmmountTransfered.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column61 = DblParser(item.ChangeValueTransfered)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column62 = item.ChangeAmmountDiscarded.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column63 = DblParser(item.ChangeValueDiscarded)

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column64 = DblParser(item.AfterAcquisitionAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column65 = DblParser(item.AfterAcquisitionAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column66 = DblParser(item.AfterAmortizationAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column67 = DblParser(item.AfterAmortizationAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column68 = DblParser(item.AfterValueDecreaseAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column69 = DblParser(item.AfterValueDecreaseAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column70 = DblParser(item.AfterValueIncreaseAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column71 = DblParser(item.AfterValueIncreaseAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column72 = DblParser(item.AfterValueIncreaseAmmortizationAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column73 = DblParser(item.AfterValueIncreaseAmmortizationAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column74 = DblParser(item.AfterValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column75 = DblParser(item.AfterValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column76 = item.AfterAmmount.ToString
        Next

        ReportFileName = "R_LongTermAssetInfoList.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="ActiveReports.LongTermAssetOperationInfoListParent">LongTermAssetOperationInfoListParent</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As ActiveReports.LongTermAssetOperationInfoListParent, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.Name
        RD.TableGeneral.Item(0).Column2 = R_Obj.MeasureUnit
        RD.TableGeneral.Item(0).Column3 = R_Obj.LegalGroup
        RD.TableGeneral.Item(0).Column4 = R_Obj.CustomGroup
        RD.TableGeneral.Item(0).Column5 = R_Obj.AcquisitionDate.ToShortDateString
        RD.TableGeneral.Item(0).Column6 = R_Obj.AcquisitionJournalEntryDocNumber
        RD.TableGeneral.Item(0).Column7 = R_Obj.AcquisitionJournalEntryDocContent
        RD.TableGeneral.Item(0).Column8 = R_Obj.InventoryNumber
        RD.TableGeneral.Item(0).Column9 = R_Obj.AccountAcquisition.ToString
        RD.TableGeneral.Item(0).Column10 = R_Obj.AccountAccumulatedAmortization.ToString
        RD.TableGeneral.Item(0).Column11 = R_Obj.AccountValueIncrease.ToString
        RD.TableGeneral.Item(0).Column12 = R_Obj.AccountValueDecrease.ToString
        RD.TableGeneral.Item(0).Column13 = R_Obj.AccountRevaluedPortionAmmortization.ToString
        RD.TableGeneral.Item(0).Column14 = DblParser(R_Obj.LiquidationUnitValue)
        If R_Obj.ContinuedUsage Then
            RD.TableGeneral.Item(0).Column15 = "Taip"
        Else
            RD.TableGeneral.Item(0).Column15 = "Ne"
        End If
        RD.TableGeneral.Item(0).Column16 = R_Obj.DefaultAmortizationPeriod.ToString
        RD.TableGeneral.Item(0).Column17 = DblParser(R_Obj.AcquisitionAccountValue)
        RD.TableGeneral.Item(0).Column18 = DblParser(R_Obj.AcquisitionAccountValuePerUnit, 4)
        RD.TableGeneral.Item(0).Column19 = DblParser(R_Obj.AmortizationAccountValue)
        RD.TableGeneral.Item(0).Column20 = DblParser(R_Obj.AmortizationAccountValuePerUnit, 4)
        RD.TableGeneral.Item(0).Column21 = DblParser(R_Obj.ValueDecreaseAccountValue)
        RD.TableGeneral.Item(0).Column22 = DblParser(R_Obj.ValueDecreaseAccountValuePerUnit, 4)
        RD.TableGeneral.Item(0).Column23 = DblParser(R_Obj.ValueIncreaseAccountValue)
        RD.TableGeneral.Item(0).Column24 = DblParser(R_Obj.ValueIncreaseAccountValuePerUnit, 4)
        RD.TableGeneral.Item(0).Column25 = DblParser(R_Obj.ValueIncreaseAmmortizationAccountValue)
        RD.TableGeneral.Item(0).Column26 = DblParser(R_Obj.ValueIncreaseAmmortizationAccountValuePerUnit, 4)
        RD.TableGeneral.Item(0).Column27 = DblParser(R_Obj.Value)
        RD.TableGeneral.Item(0).Column28 = DblParser(R_Obj.ValuePerUnit, 4)
        RD.TableGeneral.Item(0).Column29 = R_Obj.Ammount.ToString
        RD.TableGeneral.Item(0).Column30 = DblParser(R_Obj.ValueRevaluedPortion)
        RD.TableGeneral.Item(0).Column31 = DblParser(R_Obj.ValueRevaluedPortionPerUnit, 4)

        For Each item As LongTermAssetOperationInfo In R_Obj.OperationList
            RD.Table1.Rows.Add()
            If item.IsComplexAct Then
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = "Taip"
            Else
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = "Ne"
            End If
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.OperationType
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.AccountChangeType
            If EnumValueAttribute.ConvertLocalizedName(Of Assets.LtaOperationType)(item.OperationType) _
                = Assets.LtaOperationType.AccountChange Then

                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = _
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 _
                    & ", " & RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3

            Else

                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = _
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2

            End If
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = item.Date.ToShortDateString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = item.Content
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = item.ActNumber.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = item.AttachedJournalEntry
            If item.CorrespondingAccount > 0 Then
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = item.CorrespondingAccount.ToString
            Else
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = ""
            End If
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = DblParser(item.BeforeOperationAcquisitionAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = DblParser(item.BeforeOperationAcquisitionAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(item.BeforeOperationAmortizationAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = DblParser(item.BeforeOperationAmortizationAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(item.BeforeOperationValueDecreaseAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = DblParser(item.BeforeOperationValueDecreaseAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = DblParser(item.BeforeOperationValueIncreaseAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = DblParser(item.BeforeOperationValueIncreaseAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = DblParser(item.BeforeOperationValueIncreaseAmmortizationAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = DblParser(item.BeforeOperationValueIncreaseAmmortizationAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column20 = DblParser(item.BeforeOperationValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column21 = DblParser(item.BeforeOperationValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column22 = item.BeforeOperationAmmount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column23 = DblParser(item.OperationAcquisitionAccountValueChange)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column24 = DblParser(item.OperationAcquisitionAccountValueChangePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column25 = DblParser(item.OperationAmortizationAccountValueChange)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column26 = DblParser(item.OperationAmortizationAccountValueChangePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column27 = DblParser(item.OperationValueDecreaseAccountValueChange)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column28 = DblParser(item.OperationValueDecreaseAccountValueChangePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column29 = DblParser(item.OperationValueIncreaseAccountValueChange)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column30 = DblParser(item.OperationValueIncreaseAccountValueChangePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column31 = DblParser(item.OperationValueIncreaseAmmortizationAccountValueChange)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column32 = DblParser(item.OperationValueIncreaseAmmortizationAccountValueChangePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column33 = DblParser(item.OperationValueChange)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column34 = DblParser(item.OperationValueChangePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column35 = item.OperationAmmountChange.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column36 = DblParser(item.AfterOperationAcquisitionAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column37 = DblParser(item.AfterOperationAcquisitionAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column38 = DblParser(item.AfterOperationAmortizationAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column39 = DblParser(item.AfterOperationAmortizationAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column40 = DblParser(item.AfterOperationValueDecreaseAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column41 = DblParser(item.AfterOperationValueDecreaseAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column42 = DblParser(item.AfterOperationValueIncreaseAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column43 = DblParser(item.AfterOperationValueIncreaseAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column44 = DblParser(item.AfterOperationValueIncreaseAmmortizationAccountValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column45 = DblParser(item.AfterOperationValueIncreaseAmmortizationAccountValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column46 = DblParser(item.AfterOperationValue)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column47 = DblParser(item.AfterOperationValuePerUnit, 4)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column48 = item.AfterOperationAmmount.ToString
            If item.NewAmortizationPeriod > 0 Then
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column49 = item.NewAmortizationPeriod.ToString
            Else
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column49 = ""
            End If
        Next

        ReportFileName = "R_LongTermAssetOperationInfoListParent.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Assets.OperationAccountChange">OperationAccountChange</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Assets.OperationAccountChange, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.ID.ToString
        RD.TableGeneral.Item(0).Column2 = R_Obj.AccountTypeHumanReadable
        RD.TableGeneral.Item(0).Column3 = R_Obj.InsertDate.ToString("yyyy-MM-dd HH:mm:ss")
        RD.TableGeneral.Item(0).Column4 = R_Obj.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss")
        RD.TableGeneral.Item(0).Column5 = BooleanToCheckMark(R_Obj.IsComplexAct)
        RD.TableGeneral.Item(0).Column6 = R_Obj.ComplexActID.ToString
        RD.TableGeneral.Item(0).Column7 = R_Obj.Date.ToString("yyyy-MM-dd")
        RD.TableGeneral.Item(0).Column8 = R_Obj.Content
        RD.TableGeneral.Item(0).Column9 = R_Obj.DocumentNumber
        RD.TableGeneral.Item(0).Column10 = R_Obj.JournalEntryID.ToString

        RD.Table1.Rows.Add()

        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = R_Obj.Background.AssetID.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = R_Obj.Background.AssetName
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = R_Obj.Background.AssetMeasureUnit
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = R_Obj.Background.AssetDateAcquired.ToString("yyyy-MM-dd")
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = R_Obj.Background.AssetAquisitionID.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = DblParser(R_Obj.Background.AssetLiquidationValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = R_Obj.Background.InitialAssetAcquiredAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = R_Obj.Background.InitialAssetContraryAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = R_Obj.Background.InitialAssetValueDecreaseAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = R_Obj.Background.InitialAssetValueIncreaseAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = R_Obj.Background.InitialAssetValueIncreaseAmortizationAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = R_Obj.Background.CurrentAssetAcquiredAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = R_Obj.Background.CurrentAssetContraryAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = R_Obj.Background.CurrentAssetValueDecreaseAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = R_Obj.Background.CurrentAssetValueIncreaseAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = R_Obj.Background.CurrentAssetValueIncreaseAmortizationAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = DblParser(R_Obj.Background.CurrentAcquisitionAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = DblParser(R_Obj.Background.CurrentAcquisitionAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = DblParser(R_Obj.Background.CurrentAmortizationAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column20 = DblParser(R_Obj.Background.CurrentAmortizationAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column21 = DblParser(R_Obj.Background.CurrentValueDecreaseAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column22 = DblParser(R_Obj.Background.CurrentValueDecreaseAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column23 = DblParser(R_Obj.Background.CurrentValueIncreaseAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column24 = DblParser(R_Obj.Background.CurrentValueIncreaseAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column25 = DblParser(R_Obj.Background.CurrentValueIncreaseAmortizationAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column26 = DblParser(R_Obj.Background.CurrentValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column27 = R_Obj.Background.CurrentAssetAmount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column28 = DblParser(R_Obj.Background.CurrentAssetValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column29 = DblParser(R_Obj.Background.CurrentAssetValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column30 = DblParser(R_Obj.Background.CurrentAssetValueRevaluedPortion, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column31 = DblParser(R_Obj.Background.CurrentAssetValueRevaluedPortionPerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column32 = R_Obj.Background.CurrentUsageTermMonths.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column33 = R_Obj.Background.CurrentAmortizationPeriod.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column34 = BooleanToCheckMark(R_Obj.Background.CurrentUsageStatus)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column35 = R_Obj.CurrentAccount.ToString()
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column36 = DblParser(R_Obj.CurrentAccountBalance, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column37 = R_Obj.NewAccount.ToString()
        
        ReportFileName = "R_LongTermAssetAccountChange.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Assets.OperationAcquisitionValueIncrease">OperationAcquisitionValueIncrease</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Assets.OperationAcquisitionValueIncrease, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.ID.ToString
        RD.TableGeneral.Item(0).Column2 = R_Obj.InsertDate.ToString("yyyy-MM-dd HH:mm:ss")
        RD.TableGeneral.Item(0).Column3 = R_Obj.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss")
        RD.TableGeneral.Item(0).Column4 = BooleanToCheckMark(R_Obj.IsComplexAct)
        RD.TableGeneral.Item(0).Column5 = R_Obj.ComplexActID.ToString
        RD.TableGeneral.Item(0).Column6 = R_Obj.Date.ToString("yyyy-MM-dd")
        RD.TableGeneral.Item(0).Column7 = R_Obj.Content
        RD.TableGeneral.Item(0).Column8 = R_Obj.JournalEntryID.ToString
        RD.TableGeneral.Item(0).Column9 = R_Obj.JournalEntryDocumentNumber
        RD.TableGeneral.Item(0).Column10 = R_Obj.JournalEntryContent
        RD.TableGeneral.Item(0).Column11 = R_Obj.JournalEntryPerson
        RD.TableGeneral.Item(0).Column12 = R_Obj.JournalEntryDocumentType
        RD.TableGeneral.Item(0).Column13 = DblParser(R_Obj.JournalEntryAmount, 2)
        RD.TableGeneral.Item(0).Column14 = R_Obj.JournalEntryBookEntries.ToString

        RD.Table1.Rows.Add()

        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = R_Obj.Background.AssetID.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = R_Obj.Background.AssetName
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = R_Obj.Background.AssetMeasureUnit
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = R_Obj.Background.AssetDateAcquired.ToString("yyyy-MM-dd")
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = R_Obj.Background.AssetAquisitionID.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = DblParser(R_Obj.Background.AssetLiquidationValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = R_Obj.Background.CurrentAssetAcquiredAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = R_Obj.Background.CurrentAssetContraryAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = R_Obj.Background.CurrentAssetValueDecreaseAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = R_Obj.Background.CurrentAssetValueIncreaseAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = R_Obj.Background.CurrentAssetValueIncreaseAmortizationAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(R_Obj.Background.CurrentAcquisitionAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = DblParser(R_Obj.Background.CurrentAcquisitionAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(R_Obj.Background.CurrentAmortizationAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = DblParser(R_Obj.Background.CurrentAmortizationAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = DblParser(R_Obj.Background.CurrentValueDecreaseAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = DblParser(R_Obj.Background.CurrentValueDecreaseAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = DblParser(R_Obj.Background.CurrentValueIncreaseAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = DblParser(R_Obj.Background.CurrentValueIncreaseAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column20 = DblParser(R_Obj.Background.CurrentValueIncreaseAmortizationAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column21 = DblParser(R_Obj.Background.CurrentValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column22 = R_Obj.Background.CurrentAssetAmount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column23 = DblParser(R_Obj.Background.CurrentAssetValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column24 = DblParser(R_Obj.Background.CurrentAssetValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column25 = DblParser(R_Obj.Background.CurrentAssetValueRevaluedPortion, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column26 = DblParser(R_Obj.Background.CurrentAssetValueRevaluedPortionPerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column27 = R_Obj.Background.CurrentUsageTermMonths.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column28 = R_Obj.Background.CurrentAmortizationPeriod.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column29 = BooleanToCheckMark(R_Obj.Background.CurrentUsageStatus)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column30 = R_Obj.Background.ChangeAssetAmount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column31 = DblParser(R_Obj.Background.ChangeAcquisitionAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column32 = DblParser(R_Obj.Background.ChangeAcquisitionAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column33 = DblParser(R_Obj.Background.ChangeAssetUnitValue, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column34 = DblParser(R_Obj.Background.ChangeAssetValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column35 = DblParser(R_Obj.Background.ChangeAssetRevaluedPortionUnitValue, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column36 = DblParser(R_Obj.Background.ChangeAssetRevaluedPortionValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column37 = DblParser(R_Obj.Background.AfterOperationAcquisitionAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column38 = DblParser(R_Obj.Background.AfterOperationAcquisitionAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column39 = DblParser(R_Obj.Background.AfterOperationAssetValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column40 = DblParser(R_Obj.Background.AfterOperationAssetValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column41 = DblParser(R_Obj.Background.AfterOperationAssetValueRevaluedPortion, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column42 = DblParser(R_Obj.Background.AfterOperationAssetValueRevaluedPortionPerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column43 = DblParser(R_Obj.ValueIncrease, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column44 = DblParser(R_Obj.ValueIncreasePerUnit, ROUNDUNITASSET)

        ReportFileName = "R_LongTermAssetAcquisitionValueIncrease.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Assets.OperationAmortization">OperationAmortization</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Assets.OperationAmortization, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.ID.ToString
        RD.TableGeneral.Item(0).Column2 = R_Obj.InsertDate.ToString("yyyy-MM-dd HH:mm:ss")
        RD.TableGeneral.Item(0).Column3 = R_Obj.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss")
        RD.TableGeneral.Item(0).Column4 = BooleanToCheckMark(R_Obj.IsComplexAct)
        RD.TableGeneral.Item(0).Column5 = R_Obj.ComplexActID.ToString
        RD.TableGeneral.Item(0).Column6 = R_Obj.Date.ToString("yyyy-MM-dd")
        RD.TableGeneral.Item(0).Column7 = R_Obj.Content
        RD.TableGeneral.Item(0).Column8 = R_Obj.JournalEntryID.ToString
        RD.TableGeneral.Item(0).Column9 = R_Obj.DocumentNumber
        RD.TableGeneral.Item(0).Column10 = DblParser(R_Obj.TotalValueChange, 2)
        
        RD.Table1.Rows.Add()

        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = R_Obj.Background.AssetID.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = R_Obj.Background.AssetName
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = R_Obj.Background.AssetMeasureUnit
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = R_Obj.Background.AssetDateAcquired.ToString("yyyy-MM-dd")
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = R_Obj.Background.AssetAquisitionID.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = DblParser(R_Obj.Background.AssetLiquidationValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = R_Obj.Background.CurrentAssetAcquiredAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = R_Obj.Background.CurrentAssetContraryAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = R_Obj.Background.CurrentAssetValueDecreaseAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = R_Obj.Background.CurrentAssetValueIncreaseAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = R_Obj.Background.CurrentAssetValueIncreaseAmortizationAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(R_Obj.Background.CurrentAcquisitionAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = DblParser(R_Obj.Background.CurrentAcquisitionAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(R_Obj.Background.CurrentAmortizationAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = DblParser(R_Obj.Background.CurrentAmortizationAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = DblParser(R_Obj.Background.CurrentValueDecreaseAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = DblParser(R_Obj.Background.CurrentValueDecreaseAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = DblParser(R_Obj.Background.CurrentValueIncreaseAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = DblParser(R_Obj.Background.CurrentValueIncreaseAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column20 = DblParser(R_Obj.Background.CurrentValueIncreaseAmortizationAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column21 = DblParser(R_Obj.Background.CurrentValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column22 = R_Obj.Background.CurrentAssetAmount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column23 = DblParser(R_Obj.Background.CurrentAssetValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column24 = DblParser(R_Obj.Background.CurrentAssetValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column25 = DblParser(R_Obj.Background.CurrentAssetValueRevaluedPortion, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column26 = DblParser(R_Obj.Background.CurrentAssetValueRevaluedPortionPerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column27 = R_Obj.Background.CurrentUsageTermMonths.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column28 = R_Obj.Background.CurrentAmortizationPeriod.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column29 = BooleanToCheckMark(R_Obj.Background.CurrentUsageStatus)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column30 = DblParser(R_Obj.Background.ChangeAmortizationAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column31 = DblParser(R_Obj.Background.ChangeAmortizationAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column32 = DblParser(R_Obj.Background.ChangeValueIncreaseAmortizationAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column33 = DblParser(R_Obj.Background.ChangeValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column34 = DblParser(R_Obj.Background.ChangeAssetUnitValue, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column35 = DblParser(R_Obj.Background.ChangeAssetValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column36 = DblParser(R_Obj.Background.ChangeAssetRevaluedPortionUnitValue, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column37 = DblParser(R_Obj.Background.ChangeAssetRevaluedPortionValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column38 = DblParser(R_Obj.Background.AfterOperationAmortizationAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column39 = DblParser(R_Obj.Background.AfterOperationAmortizationAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column40 = DblParser(R_Obj.Background.AfterOperationValueIncreaseAmortizationAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column41 = DblParser(R_Obj.Background.AfterOperationValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column42 = DblParser(R_Obj.Background.AfterOperationAssetValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column43 = DblParser(R_Obj.Background.AfterOperationAssetValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column44 = DblParser(R_Obj.Background.AfterOperationAssetValueRevaluedPortion, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column45 = DblParser(R_Obj.Background.AfterOperationAssetValueRevaluedPortionPerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column46 = DblParser(R_Obj.TotalValueChange, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column47 = DblParser(R_Obj.UnitValueChange, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column48 = DblParser(R_Obj.RevaluedPortionTotalValueChange, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column49 = DblParser(R_Obj.RevaluedPortionUnitValueChange, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column50 = R_Obj.AmortizationCalculatedForMonths.ToString()
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column51 = R_Obj.AmortizationCalculations
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column52 = R_Obj.AccountCosts.ToString()

        ReportFileName = "R_LongTermAssetAmortization.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Assets.ComplexOperationAmortization">ComplexOperationAmortization</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Assets.ComplexOperationAmortization, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.ID.ToString
        RD.TableGeneral.Item(0).Column2 = R_Obj.InsertDate.ToString("yyyy-MM-dd HH:mm:ss")
        RD.TableGeneral.Item(0).Column3 = R_Obj.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss")
        RD.TableGeneral.Item(0).Column4 = BooleanToCheckMark(True)
        RD.TableGeneral.Item(0).Column5 = R_Obj.ID.ToString
        RD.TableGeneral.Item(0).Column6 = R_Obj.Date.ToString("yyyy-MM-dd")
        RD.TableGeneral.Item(0).Column7 = R_Obj.Content
        RD.TableGeneral.Item(0).Column8 = R_Obj.JournalEntryID.ToString
        RD.TableGeneral.Item(0).Column9 = R_Obj.DocumentNumber
        RD.TableGeneral.Item(0).Column10 = DblParser(R_Obj.TotalValueChange, 2)

        For Each item As Assets.OperationAmortization In R_Obj.ItemsSorted

            RD.Table1.Rows.Add()

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.Background.AssetID.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.Background.AssetName
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.Background.AssetMeasureUnit
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.Background.AssetDateAcquired.ToString("yyyy-MM-dd")
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = item.Background.AssetAquisitionID.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = DblParser(item.Background.AssetLiquidationValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = item.Background.CurrentAssetAcquiredAccount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = item.Background.CurrentAssetContraryAccount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = item.Background.CurrentAssetValueDecreaseAccount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = item.Background.CurrentAssetValueIncreaseAccount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = item.Background.CurrentAssetValueIncreaseAmortizationAccount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(item.Background.CurrentAcquisitionAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = DblParser(item.Background.CurrentAcquisitionAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(item.Background.CurrentAmortizationAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = DblParser(item.Background.CurrentAmortizationAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = DblParser(item.Background.CurrentValueDecreaseAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = DblParser(item.Background.CurrentValueDecreaseAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = DblParser(item.Background.CurrentValueIncreaseAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = DblParser(item.Background.CurrentValueIncreaseAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column20 = DblParser(item.Background.CurrentValueIncreaseAmortizationAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column21 = DblParser(item.Background.CurrentValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column22 = item.Background.CurrentAssetAmount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column23 = DblParser(item.Background.CurrentAssetValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column24 = DblParser(item.Background.CurrentAssetValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column25 = DblParser(item.Background.CurrentAssetValueRevaluedPortion, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column26 = DblParser(item.Background.CurrentAssetValueRevaluedPortionPerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column27 = item.Background.CurrentUsageTermMonths.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column28 = item.Background.CurrentAmortizationPeriod.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column29 = BooleanToCheckMark(item.Background.CurrentUsageStatus)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column30 = DblParser(item.Background.ChangeAmortizationAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column31 = DblParser(item.Background.ChangeAmortizationAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column32 = DblParser(item.Background.ChangeValueIncreaseAmortizationAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column33 = DblParser(item.Background.ChangeValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column34 = DblParser(item.Background.ChangeAssetUnitValue, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column35 = DblParser(item.Background.ChangeAssetValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column36 = DblParser(item.Background.ChangeAssetRevaluedPortionUnitValue, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column37 = DblParser(item.Background.ChangeAssetRevaluedPortionValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column38 = DblParser(item.Background.AfterOperationAmortizationAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column39 = DblParser(item.Background.AfterOperationAmortizationAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column40 = DblParser(item.Background.AfterOperationValueIncreaseAmortizationAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column41 = DblParser(item.Background.AfterOperationValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column42 = DblParser(item.Background.AfterOperationAssetValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column43 = DblParser(item.Background.AfterOperationAssetValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column44 = DblParser(item.Background.AfterOperationAssetValueRevaluedPortion, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column45 = DblParser(item.Background.AfterOperationAssetValueRevaluedPortionPerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column46 = DblParser(item.TotalValueChange, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column47 = DblParser(item.UnitValueChange, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column48 = DblParser(item.RevaluedPortionTotalValueChange, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column49 = DblParser(item.RevaluedPortionUnitValueChange, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column50 = item.AmortizationCalculatedForMonths.ToString()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column51 = item.AmortizationCalculations
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column52 = item.AccountCosts.ToString()

        Next

        ReportFileName = "R_LongTermAssetAmortization.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Assets.OperationAmortizationPeriodChange">OperationAmortizationPeriodChange</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Assets.OperationAmortizationPeriodChange, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.ID.ToString
        RD.TableGeneral.Item(0).Column2 = R_Obj.InsertDate.ToString("yyyy-MM-dd HH:mm:ss")
        RD.TableGeneral.Item(0).Column3 = R_Obj.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss")
        RD.TableGeneral.Item(0).Column4 = BooleanToCheckMark(R_Obj.IsComplexAct)
        RD.TableGeneral.Item(0).Column5 = R_Obj.ComplexActID.ToString
        RD.TableGeneral.Item(0).Column6 = R_Obj.Date.ToString("yyyy-MM-dd")
        RD.TableGeneral.Item(0).Column7 = R_Obj.Content
        RD.TableGeneral.Item(0).Column8 = R_Obj.DocumentNumber

        RD.Table1.Rows.Add()

        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = R_Obj.Background.AssetID.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = R_Obj.Background.AssetName
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = R_Obj.Background.AssetMeasureUnit
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = R_Obj.Background.AssetDateAcquired.ToString("yyyy-MM-dd")
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = R_Obj.Background.AssetAquisitionID.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = DblParser(R_Obj.Background.AssetLiquidationValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = R_Obj.Background.CurrentAssetAcquiredAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = R_Obj.Background.CurrentAssetContraryAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = R_Obj.Background.CurrentAssetValueDecreaseAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = R_Obj.Background.CurrentAssetValueIncreaseAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = R_Obj.Background.CurrentAssetValueIncreaseAmortizationAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(R_Obj.Background.CurrentAcquisitionAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = DblParser(R_Obj.Background.CurrentAcquisitionAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(R_Obj.Background.CurrentAmortizationAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = DblParser(R_Obj.Background.CurrentAmortizationAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = DblParser(R_Obj.Background.CurrentValueDecreaseAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = DblParser(R_Obj.Background.CurrentValueDecreaseAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = DblParser(R_Obj.Background.CurrentValueIncreaseAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = DblParser(R_Obj.Background.CurrentValueIncreaseAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column20 = DblParser(R_Obj.Background.CurrentValueIncreaseAmortizationAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column21 = DblParser(R_Obj.Background.CurrentValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column22 = R_Obj.Background.CurrentAssetAmount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column23 = DblParser(R_Obj.Background.CurrentAssetValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column24 = DblParser(R_Obj.Background.CurrentAssetValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column25 = DblParser(R_Obj.Background.CurrentAssetValueRevaluedPortion, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column26 = DblParser(R_Obj.Background.CurrentAssetValueRevaluedPortionPerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column27 = R_Obj.Background.CurrentUsageTermMonths.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column28 = R_Obj.Background.CurrentAmortizationPeriod.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column29 = BooleanToCheckMark(R_Obj.Background.CurrentUsageStatus)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column30 = R_Obj.NewPeriod.ToString

        ReportFileName = "R_LongTermAssetAmortizationPeriod.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Assets.OperationDiscard">OperationDiscard</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Assets.OperationDiscard, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.ID.ToString
        RD.TableGeneral.Item(0).Column2 = R_Obj.InsertDate.ToString("yyyy-MM-dd HH:mm:ss")
        RD.TableGeneral.Item(0).Column3 = R_Obj.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss")
        RD.TableGeneral.Item(0).Column4 = BooleanToCheckMark(R_Obj.IsComplexAct)
        RD.TableGeneral.Item(0).Column5 = R_Obj.ComplexActID.ToString
        RD.TableGeneral.Item(0).Column6 = R_Obj.Date.ToString("yyyy-MM-dd")
        RD.TableGeneral.Item(0).Column7 = R_Obj.Content
        RD.TableGeneral.Item(0).Column8 = R_Obj.JournalEntryID.ToString
        RD.TableGeneral.Item(0).Column9 = R_Obj.DocumentNumber
        RD.TableGeneral.Item(0).Column10 = DblParser(-R_Obj.Background.ChangeAssetValue, 2)

        RD.Table1.Rows.Add()

        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = R_Obj.Background.AssetID.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = R_Obj.Background.AssetName
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = R_Obj.Background.AssetMeasureUnit
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = R_Obj.Background.AssetDateAcquired.ToString("yyyy-MM-dd")
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = R_Obj.Background.AssetAquisitionID.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = DblParser(R_Obj.Background.AssetLiquidationValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = R_Obj.Background.CurrentAssetAcquiredAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = R_Obj.Background.CurrentAssetContraryAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = R_Obj.Background.CurrentAssetValueDecreaseAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = R_Obj.Background.CurrentAssetValueIncreaseAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = R_Obj.Background.CurrentAssetValueIncreaseAmortizationAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(R_Obj.Background.CurrentAcquisitionAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = DblParser(R_Obj.Background.CurrentAcquisitionAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(R_Obj.Background.CurrentAmortizationAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = DblParser(R_Obj.Background.CurrentAmortizationAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = DblParser(R_Obj.Background.CurrentValueDecreaseAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = DblParser(R_Obj.Background.CurrentValueDecreaseAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = DblParser(R_Obj.Background.CurrentValueIncreaseAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = DblParser(R_Obj.Background.CurrentValueIncreaseAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column20 = DblParser(R_Obj.Background.CurrentValueIncreaseAmortizationAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column21 = DblParser(R_Obj.Background.CurrentValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column22 = R_Obj.Background.CurrentAssetAmount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column23 = DblParser(R_Obj.Background.CurrentAssetValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column24 = DblParser(R_Obj.Background.CurrentAssetValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column25 = DblParser(R_Obj.Background.CurrentAssetValueRevaluedPortion, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column26 = DblParser(R_Obj.Background.CurrentAssetValueRevaluedPortionPerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column27 = R_Obj.Background.CurrentUsageTermMonths.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column28 = R_Obj.Background.CurrentAmortizationPeriod.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column29 = BooleanToCheckMark(R_Obj.Background.CurrentUsageStatus)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column30 = R_Obj.Background.ChangeAssetAmount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column31 = DblParser(-R_Obj.Background.ChangeAcquisitionAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column32 = DblParser(-R_Obj.Background.ChangeAcquisitionAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column33 = DblParser(-R_Obj.Background.ChangeAmortizationAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column34 = DblParser(-R_Obj.Background.ChangeAmortizationAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column35 = DblParser(-R_Obj.Background.ChangeValueDecreaseAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column36 = DblParser(-R_Obj.Background.ChangeValueDecreaseAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column37 = DblParser(-R_Obj.Background.ChangeValueIncreaseAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column38 = DblParser(-R_Obj.Background.ChangeValueIncreaseAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column39 = DblParser(-R_Obj.Background.ChangeValueIncreaseAmortizationAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column40 = DblParser(-R_Obj.Background.ChangeValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column41 = DblParser(-R_Obj.Background.ChangeAssetValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column42 = DblParser(-R_Obj.Background.ChangeAssetRevaluedPortionValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column43 = DblParser(R_Obj.Background.AfterOperationAcquisitionAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column44 = DblParser(R_Obj.Background.AfterOperationAcquisitionAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column45 = DblParser(R_Obj.Background.AfterOperationAmortizationAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column46 = DblParser(R_Obj.Background.AfterOperationAmortizationAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column47 = DblParser(R_Obj.Background.AfterOperationValueDecreaseAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column48 = DblParser(R_Obj.Background.AfterOperationValueDecreaseAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column49 = DblParser(R_Obj.Background.AfterOperationValueIncreaseAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column50 = DblParser(R_Obj.Background.AfterOperationValueIncreaseAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column51 = DblParser(R_Obj.Background.AfterOperationValueIncreaseAmortizationAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column52 = DblParser(R_Obj.Background.AfterOperationValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column53 = R_Obj.Background.AfterOperationAssetAmount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column54 = DblParser(R_Obj.Background.AfterOperationAssetValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column55 = DblParser(R_Obj.Background.AfterOperationAssetValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column56 = DblParser(R_Obj.Background.AfterOperationAssetValueRevaluedPortion, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column57 = DblParser(R_Obj.Background.AfterOperationAssetValueRevaluedPortionPerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column58 = R_Obj.AmountToDiscard.ToString()
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column59 = R_Obj.AccountCosts.ToString()

        ReportFileName = "R_LongTermAssetDiscard.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Assets.ComplexOperationDiscard">ComplexOperationDiscard</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Assets.ComplexOperationDiscard, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.ID.ToString
        RD.TableGeneral.Item(0).Column2 = R_Obj.InsertDate.ToString("yyyy-MM-dd HH:mm:ss")
        RD.TableGeneral.Item(0).Column3 = R_Obj.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss")
        RD.TableGeneral.Item(0).Column4 = BooleanToCheckMark(True)
        RD.TableGeneral.Item(0).Column5 = R_Obj.ID.ToString
        RD.TableGeneral.Item(0).Column6 = R_Obj.Date.ToString("yyyy-MM-dd")
        RD.TableGeneral.Item(0).Column7 = R_Obj.Content
        RD.TableGeneral.Item(0).Column8 = R_Obj.JournalEntryID.ToString
        RD.TableGeneral.Item(0).Column9 = R_Obj.DocumentNumber
        RD.TableGeneral.Item(0).Column10 = DblParser(R_Obj.TotalDiscardCosts, 2)

        For Each item As Assets.OperationDiscard In R_Obj.ItemsSorted

            RD.Table1.Rows.Add()

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.Background.AssetID.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.Background.AssetName
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.Background.AssetMeasureUnit
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.Background.AssetDateAcquired.ToString("yyyy-MM-dd")
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = item.Background.AssetAquisitionID.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = DblParser(item.Background.AssetLiquidationValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = item.Background.CurrentAssetAcquiredAccount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = item.Background.CurrentAssetContraryAccount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = item.Background.CurrentAssetValueDecreaseAccount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = item.Background.CurrentAssetValueIncreaseAccount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = item.Background.CurrentAssetValueIncreaseAmortizationAccount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(item.Background.CurrentAcquisitionAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = DblParser(item.Background.CurrentAcquisitionAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(item.Background.CurrentAmortizationAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = DblParser(item.Background.CurrentAmortizationAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = DblParser(item.Background.CurrentValueDecreaseAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = DblParser(item.Background.CurrentValueDecreaseAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = DblParser(item.Background.CurrentValueIncreaseAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = DblParser(item.Background.CurrentValueIncreaseAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column20 = DblParser(item.Background.CurrentValueIncreaseAmortizationAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column21 = DblParser(item.Background.CurrentValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column22 = item.Background.CurrentAssetAmount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column23 = DblParser(item.Background.CurrentAssetValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column24 = DblParser(item.Background.CurrentAssetValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column25 = DblParser(item.Background.CurrentAssetValueRevaluedPortion, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column26 = DblParser(item.Background.CurrentAssetValueRevaluedPortionPerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column27 = item.Background.CurrentUsageTermMonths.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column28 = item.Background.CurrentAmortizationPeriod.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column29 = BooleanToCheckMark(item.Background.CurrentUsageStatus)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column30 = item.Background.ChangeAssetAmount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column31 = DblParser(-item.Background.ChangeAcquisitionAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column32 = DblParser(-item.Background.ChangeAcquisitionAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column33 = DblParser(-item.Background.ChangeAmortizationAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column34 = DblParser(-item.Background.ChangeAmortizationAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column35 = DblParser(-item.Background.ChangeValueDecreaseAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column36 = DblParser(-item.Background.ChangeValueDecreaseAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column37 = DblParser(-item.Background.ChangeValueIncreaseAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column38 = DblParser(-item.Background.ChangeValueIncreaseAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column39 = DblParser(-item.Background.ChangeValueIncreaseAmortizationAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column40 = DblParser(-item.Background.ChangeValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column41 = DblParser(-item.Background.ChangeAssetValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column42 = DblParser(-item.Background.ChangeAssetRevaluedPortionValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column43 = DblParser(item.Background.AfterOperationAcquisitionAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column44 = DblParser(item.Background.AfterOperationAcquisitionAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column45 = DblParser(item.Background.AfterOperationAmortizationAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column46 = DblParser(item.Background.AfterOperationAmortizationAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column47 = DblParser(item.Background.AfterOperationValueDecreaseAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column48 = DblParser(item.Background.AfterOperationValueDecreaseAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column49 = DblParser(item.Background.AfterOperationValueIncreaseAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column50 = DblParser(item.Background.AfterOperationValueIncreaseAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column51 = DblParser(item.Background.AfterOperationValueIncreaseAmortizationAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column52 = DblParser(item.Background.AfterOperationValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column53 = item.Background.AfterOperationAssetAmount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column54 = DblParser(item.Background.AfterOperationAssetValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column55 = DblParser(item.Background.AfterOperationAssetValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column56 = DblParser(item.Background.AfterOperationAssetValueRevaluedPortion, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column57 = DblParser(item.Background.AfterOperationAssetValueRevaluedPortionPerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column58 = item.AmountToDiscard.ToString()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column59 = item.AccountCosts.ToString()

        Next

        ReportFileName = "R_LongTermAssetDiscard.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Assets.OperationOperationalStatusChange">OperationOperationalStatusChange</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Assets.OperationOperationalStatusChange, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.ID.ToString
        RD.TableGeneral.Item(0).Column2 = R_Obj.InsertDate.ToString("yyyy-MM-dd HH:mm:ss")
        RD.TableGeneral.Item(0).Column3 = R_Obj.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss")
        RD.TableGeneral.Item(0).Column4 = BooleanToCheckMark(R_Obj.IsComplexAct)
        RD.TableGeneral.Item(0).Column5 = R_Obj.ComplexActID.ToString
        RD.TableGeneral.Item(0).Column6 = R_Obj.Date.ToString("yyyy-MM-dd")
        RD.TableGeneral.Item(0).Column7 = R_Obj.Content
        RD.TableGeneral.Item(0).Column8 = R_Obj.DocumentNumber

        RD.Table1.Rows.Add()

        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = R_Obj.Background.AssetID.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = R_Obj.Background.AssetName
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = R_Obj.Background.AssetMeasureUnit
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = R_Obj.Background.AssetDateAcquired.ToString("yyyy-MM-dd")
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = R_Obj.Background.AssetAquisitionID.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = DblParser(R_Obj.Background.AssetLiquidationValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = R_Obj.Background.CurrentAssetAcquiredAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = R_Obj.Background.CurrentAssetContraryAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = R_Obj.Background.CurrentAssetValueDecreaseAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = R_Obj.Background.CurrentAssetValueIncreaseAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = R_Obj.Background.CurrentAssetValueIncreaseAmortizationAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(R_Obj.Background.CurrentAcquisitionAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = DblParser(R_Obj.Background.CurrentAcquisitionAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(R_Obj.Background.CurrentAmortizationAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = DblParser(R_Obj.Background.CurrentAmortizationAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = DblParser(R_Obj.Background.CurrentValueDecreaseAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = DblParser(R_Obj.Background.CurrentValueDecreaseAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = DblParser(R_Obj.Background.CurrentValueIncreaseAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = DblParser(R_Obj.Background.CurrentValueIncreaseAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column20 = DblParser(R_Obj.Background.CurrentValueIncreaseAmortizationAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column21 = DblParser(R_Obj.Background.CurrentValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column22 = R_Obj.Background.CurrentAssetAmount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column23 = DblParser(R_Obj.Background.CurrentAssetValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column24 = DblParser(R_Obj.Background.CurrentAssetValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column25 = DblParser(R_Obj.Background.CurrentAssetValueRevaluedPortion, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column26 = DblParser(R_Obj.Background.CurrentAssetValueRevaluedPortionPerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column27 = R_Obj.Background.CurrentUsageTermMonths.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column28 = R_Obj.Background.CurrentAmortizationPeriod.ToString

        If R_Obj.BeginOperationalPeriod Then
            ReportFileName = "R_LongTermAssetUsingStart.rdlc"
        Else
            ReportFileName = "R_LongTermAssetUsingEnd.rdlc"
        End If

        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Assets.ComplexOperationOperationalStatusChange">ComplexOperationOperationalStatusChange</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Assets.ComplexOperationOperationalStatusChange, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.ID.ToString
        RD.TableGeneral.Item(0).Column2 = R_Obj.InsertDate.ToString("yyyy-MM-dd HH:mm:ss")
        RD.TableGeneral.Item(0).Column3 = R_Obj.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss")
        RD.TableGeneral.Item(0).Column4 = BooleanToCheckMark(True)
        RD.TableGeneral.Item(0).Column5 = R_Obj.ID.ToString
        RD.TableGeneral.Item(0).Column6 = R_Obj.Date.ToString("yyyy-MM-dd")
        RD.TableGeneral.Item(0).Column7 = R_Obj.Content
        RD.TableGeneral.Item(0).Column8 = R_Obj.DocumentNumber

        For Each item As Assets.OperationOperationalStatusChange In R_Obj.ItemsSorted

            RD.Table1.Rows.Add()

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.Background.AssetID.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.Background.AssetName
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.Background.AssetMeasureUnit
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.Background.AssetDateAcquired.ToString("yyyy-MM-dd")
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = item.Background.AssetAquisitionID.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = DblParser(item.Background.AssetLiquidationValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = item.Background.CurrentAssetAcquiredAccount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = item.Background.CurrentAssetContraryAccount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = item.Background.CurrentAssetValueDecreaseAccount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = item.Background.CurrentAssetValueIncreaseAccount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = item.Background.CurrentAssetValueIncreaseAmortizationAccount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(item.Background.CurrentAcquisitionAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = DblParser(item.Background.CurrentAcquisitionAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(item.Background.CurrentAmortizationAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = DblParser(item.Background.CurrentAmortizationAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = DblParser(item.Background.CurrentValueDecreaseAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = DblParser(item.Background.CurrentValueDecreaseAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = DblParser(item.Background.CurrentValueIncreaseAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = DblParser(item.Background.CurrentValueIncreaseAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column20 = DblParser(item.Background.CurrentValueIncreaseAmortizationAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column21 = DblParser(item.Background.CurrentValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column22 = item.Background.CurrentAssetAmount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column23 = DblParser(item.Background.CurrentAssetValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column24 = DblParser(item.Background.CurrentAssetValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column25 = DblParser(item.Background.CurrentAssetValueRevaluedPortion, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column26 = DblParser(item.Background.CurrentAssetValueRevaluedPortionPerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column27 = item.Background.CurrentUsageTermMonths.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column28 = item.Background.CurrentAmortizationPeriod.ToString

        Next

        If R_Obj.BeginOperationalPeriod Then
            ReportFileName = "R_LongTermAssetUsingStart.rdlc"
        Else
            ReportFileName = "R_LongTermAssetUsingEnd.rdlc"
        End If

        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Assets.OperationTransfer">OperationTransfer</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Assets.OperationTransfer, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.ID.ToString
        RD.TableGeneral.Item(0).Column2 = R_Obj.InsertDate.ToString("yyyy-MM-dd HH:mm:ss")
        RD.TableGeneral.Item(0).Column3 = R_Obj.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss")
        RD.TableGeneral.Item(0).Column4 = BooleanToCheckMark(R_Obj.IsComplexAct)
        RD.TableGeneral.Item(0).Column5 = R_Obj.ComplexActID.ToString
        RD.TableGeneral.Item(0).Column6 = R_Obj.Date.ToString("yyyy-MM-dd")
        RD.TableGeneral.Item(0).Column7 = R_Obj.Content
        RD.TableGeneral.Item(0).Column8 = R_Obj.JournalEntryID.ToString
        RD.TableGeneral.Item(0).Column9 = R_Obj.JournalEntryDocumentNumber
        RD.TableGeneral.Item(0).Column10 = R_Obj.JournalEntryContent
        RD.TableGeneral.Item(0).Column11 = R_Obj.JournalEntryPerson
        RD.TableGeneral.Item(0).Column12 = R_Obj.JournalEntryDocumentType
        RD.TableGeneral.Item(0).Column13 = DblParser(R_Obj.JournalEntryAmount, 2)
        RD.TableGeneral.Item(0).Column14 = R_Obj.JournalEntryBookEntries

        RD.Table1.Rows.Add()

        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = R_Obj.Background.AssetID.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = R_Obj.Background.AssetName
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = R_Obj.Background.AssetMeasureUnit
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = R_Obj.Background.AssetDateAcquired.ToString("yyyy-MM-dd")
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = R_Obj.Background.AssetAquisitionID.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = DblParser(R_Obj.Background.AssetLiquidationValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = R_Obj.Background.CurrentAssetAcquiredAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = R_Obj.Background.CurrentAssetContraryAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = R_Obj.Background.CurrentAssetValueDecreaseAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = R_Obj.Background.CurrentAssetValueIncreaseAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = R_Obj.Background.CurrentAssetValueIncreaseAmortizationAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(R_Obj.Background.CurrentAcquisitionAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = DblParser(R_Obj.Background.CurrentAcquisitionAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(R_Obj.Background.CurrentAmortizationAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = DblParser(R_Obj.Background.CurrentAmortizationAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = DblParser(R_Obj.Background.CurrentValueDecreaseAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = DblParser(R_Obj.Background.CurrentValueDecreaseAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = DblParser(R_Obj.Background.CurrentValueIncreaseAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = DblParser(R_Obj.Background.CurrentValueIncreaseAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column20 = DblParser(R_Obj.Background.CurrentValueIncreaseAmortizationAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column21 = DblParser(R_Obj.Background.CurrentValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column22 = R_Obj.Background.CurrentAssetAmount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column23 = DblParser(R_Obj.Background.CurrentAssetValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column24 = DblParser(R_Obj.Background.CurrentAssetValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column25 = DblParser(R_Obj.Background.CurrentAssetValueRevaluedPortion, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column26 = DblParser(R_Obj.Background.CurrentAssetValueRevaluedPortionPerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column27 = R_Obj.Background.CurrentUsageTermMonths.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column28 = R_Obj.Background.CurrentAmortizationPeriod.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column29 = BooleanToCheckMark(R_Obj.Background.CurrentUsageStatus)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column30 = R_Obj.Background.ChangeAssetAmount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column31 = DblParser(-R_Obj.Background.ChangeAcquisitionAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column32 = DblParser(-R_Obj.Background.ChangeAcquisitionAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column33 = DblParser(-R_Obj.Background.ChangeAmortizationAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column34 = DblParser(-R_Obj.Background.ChangeAmortizationAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column35 = DblParser(-R_Obj.Background.ChangeValueDecreaseAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column36 = DblParser(-R_Obj.Background.ChangeValueDecreaseAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column37 = DblParser(-R_Obj.Background.ChangeValueIncreaseAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column38 = DblParser(-R_Obj.Background.ChangeValueIncreaseAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column39 = DblParser(-R_Obj.Background.ChangeValueIncreaseAmortizationAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column40 = DblParser(-R_Obj.Background.ChangeValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column41 = DblParser(-R_Obj.Background.ChangeAssetValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column42 = DblParser(-R_Obj.Background.ChangeAssetRevaluedPortionValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column43 = DblParser(R_Obj.Background.AfterOperationAcquisitionAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column44 = DblParser(R_Obj.Background.AfterOperationAcquisitionAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column45 = DblParser(R_Obj.Background.AfterOperationAmortizationAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column46 = DblParser(R_Obj.Background.AfterOperationAmortizationAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column47 = DblParser(R_Obj.Background.AfterOperationValueDecreaseAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column48 = DblParser(R_Obj.Background.AfterOperationValueDecreaseAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column49 = DblParser(R_Obj.Background.AfterOperationValueIncreaseAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column50 = DblParser(R_Obj.Background.AfterOperationValueIncreaseAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column51 = DblParser(R_Obj.Background.AfterOperationValueIncreaseAmortizationAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column52 = DblParser(R_Obj.Background.AfterOperationValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column53 = R_Obj.Background.AfterOperationAssetAmount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column54 = DblParser(R_Obj.Background.AfterOperationAssetValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column55 = DblParser(R_Obj.Background.AfterOperationAssetValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column56 = DblParser(R_Obj.Background.AfterOperationAssetValueRevaluedPortion, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column57 = DblParser(R_Obj.Background.AfterOperationAssetValueRevaluedPortionPerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column58 = R_Obj.AmountToTransfer.ToString()

        ReportFileName = "R_LongTermAssetTransfer.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Assets.OperationValueChange">OperationValueChange</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Assets.OperationValueChange, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.ID.ToString
        RD.TableGeneral.Item(0).Column2 = R_Obj.InsertDate.ToString("yyyy-MM-dd HH:mm:ss")
        RD.TableGeneral.Item(0).Column3 = R_Obj.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss")
        RD.TableGeneral.Item(0).Column4 = BooleanToCheckMark(R_Obj.IsComplexAct)
        RD.TableGeneral.Item(0).Column5 = R_Obj.ComplexActID.ToString
        RD.TableGeneral.Item(0).Column6 = R_Obj.Date.ToString("yyyy-MM-dd")
        RD.TableGeneral.Item(0).Column7 = R_Obj.Content
        RD.TableGeneral.Item(0).Column8 = R_Obj.JournalEntryID.ToString
        RD.TableGeneral.Item(0).Column9 = R_Obj.JournalEntryDocumentNumber
        RD.TableGeneral.Item(0).Column10 = R_Obj.JournalEntryContent
        RD.TableGeneral.Item(0).Column11 = R_Obj.JournalEntryPerson
        RD.TableGeneral.Item(0).Column12 = R_Obj.JournalEntryDocumentType
        RD.TableGeneral.Item(0).Column13 = DblParser(R_Obj.JournalEntryAmount, 2)
        RD.TableGeneral.Item(0).Column14 = R_Obj.JournalEntryBookEntries.ToString

        RD.Table1.Rows.Add()

        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = R_Obj.Background.AssetID.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = R_Obj.Background.AssetName
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = R_Obj.Background.AssetMeasureUnit
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = R_Obj.Background.AssetDateAcquired.ToString("yyyy-MM-dd")
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = R_Obj.Background.AssetAquisitionID.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = DblParser(R_Obj.Background.AssetLiquidationValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = R_Obj.Background.CurrentAssetAcquiredAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = R_Obj.Background.CurrentAssetContraryAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = R_Obj.Background.CurrentAssetValueDecreaseAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = R_Obj.Background.CurrentAssetValueIncreaseAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = R_Obj.Background.CurrentAssetValueIncreaseAmortizationAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(R_Obj.Background.CurrentAcquisitionAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = DblParser(R_Obj.Background.CurrentAcquisitionAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(R_Obj.Background.CurrentAmortizationAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = DblParser(R_Obj.Background.CurrentAmortizationAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = DblParser(R_Obj.Background.CurrentValueDecreaseAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = DblParser(R_Obj.Background.CurrentValueDecreaseAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = DblParser(R_Obj.Background.CurrentValueIncreaseAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = DblParser(R_Obj.Background.CurrentValueIncreaseAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column20 = DblParser(R_Obj.Background.CurrentValueIncreaseAmortizationAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column21 = DblParser(R_Obj.Background.CurrentValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column22 = R_Obj.Background.CurrentAssetAmount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column23 = DblParser(R_Obj.Background.CurrentAssetValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column24 = DblParser(R_Obj.Background.CurrentAssetValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column25 = DblParser(R_Obj.Background.CurrentAssetValueRevaluedPortion, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column26 = DblParser(R_Obj.Background.CurrentAssetValueRevaluedPortionPerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column27 = R_Obj.Background.CurrentUsageTermMonths.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column28 = R_Obj.Background.CurrentAmortizationPeriod.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column29 = BooleanToCheckMark(R_Obj.Background.CurrentUsageStatus)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column30 = DblParser(R_Obj.Background.ChangeValueDecreaseAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column31 = DblParser(R_Obj.Background.ChangeValueDecreaseAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column32 = DblParser(R_Obj.Background.ChangeValueIncreaseAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column33 = DblParser(R_Obj.Background.ChangeValueIncreaseAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column34 = DblParser(R_Obj.Background.ChangeAssetUnitValue, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column35 = DblParser(R_Obj.Background.ChangeAssetValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column36 = DblParser(R_Obj.Background.ChangeAssetRevaluedPortionUnitValue, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column37 = DblParser(R_Obj.Background.ChangeAssetRevaluedPortionValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column38 = DblParser(R_Obj.Background.AfterOperationValueDecreaseAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column39 = DblParser(R_Obj.Background.AfterOperationValueDecreaseAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column40 = DblParser(R_Obj.Background.AfterOperationValueIncreaseAccountValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column41 = DblParser(R_Obj.Background.AfterOperationValueIncreaseAccountValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column42 = DblParser(R_Obj.Background.AfterOperationAssetValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column43 = DblParser(R_Obj.Background.AfterOperationAssetValuePerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column44 = DblParser(R_Obj.Background.AfterOperationAssetValueRevaluedPortion, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column45 = DblParser(R_Obj.Background.AfterOperationAssetValueRevaluedPortionPerUnit, ROUNDUNITASSET)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column46 = DblParser(R_Obj.ValueChangeTotal, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column47 = DblParser(R_Obj.ValueChangePerUnit, ROUNDUNITASSET)

        ReportFileName = "R_LongTermAssetValueChange.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Assets.ComplexOperationValueChange">ComplexOperationValueChange</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Assets.ComplexOperationValueChange, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.ID.ToString
        RD.TableGeneral.Item(0).Column2 = R_Obj.InsertDate.ToString("yyyy-MM-dd HH:mm:ss")
        RD.TableGeneral.Item(0).Column3 = R_Obj.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss")
        RD.TableGeneral.Item(0).Column4 = BooleanToCheckMark(True)
        RD.TableGeneral.Item(0).Column5 = R_Obj.ID.ToString
        RD.TableGeneral.Item(0).Column6 = R_Obj.Date.ToString("yyyy-MM-dd")
        RD.TableGeneral.Item(0).Column7 = R_Obj.Content
        RD.TableGeneral.Item(0).Column8 = R_Obj.JournalEntryID.ToString
        RD.TableGeneral.Item(0).Column9 = R_Obj.JournalEntryDocumentNumber
        RD.TableGeneral.Item(0).Column10 = R_Obj.JournalEntryContent
        RD.TableGeneral.Item(0).Column11 = R_Obj.JournalEntryPerson
        RD.TableGeneral.Item(0).Column12 = R_Obj.JournalEntryDocumentType
        RD.TableGeneral.Item(0).Column13 = DblParser(R_Obj.JournalEntryAmount, 2)
        RD.TableGeneral.Item(0).Column14 = R_Obj.JournalEntryBookEntries.ToString

        For Each item As Assets.OperationValueChange In R_Obj.ItemsSorted

            RD.Table1.Rows.Add()

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.Background.AssetID.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.Background.AssetName
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.Background.AssetMeasureUnit
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.Background.AssetDateAcquired.ToString("yyyy-MM-dd")
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = item.Background.AssetAquisitionID.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = DblParser(item.Background.AssetLiquidationValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = item.Background.CurrentAssetAcquiredAccount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = item.Background.CurrentAssetContraryAccount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = item.Background.CurrentAssetValueDecreaseAccount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = item.Background.CurrentAssetValueIncreaseAccount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = item.Background.CurrentAssetValueIncreaseAmortizationAccount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(item.Background.CurrentAcquisitionAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = DblParser(item.Background.CurrentAcquisitionAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(item.Background.CurrentAmortizationAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = DblParser(item.Background.CurrentAmortizationAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = DblParser(item.Background.CurrentValueDecreaseAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = DblParser(item.Background.CurrentValueDecreaseAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = DblParser(item.Background.CurrentValueIncreaseAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = DblParser(item.Background.CurrentValueIncreaseAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column20 = DblParser(item.Background.CurrentValueIncreaseAmortizationAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column21 = DblParser(item.Background.CurrentValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column22 = item.Background.CurrentAssetAmount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column23 = DblParser(item.Background.CurrentAssetValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column24 = DblParser(item.Background.CurrentAssetValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column25 = DblParser(item.Background.CurrentAssetValueRevaluedPortion, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column26 = DblParser(item.Background.CurrentAssetValueRevaluedPortionPerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column27 = item.Background.CurrentUsageTermMonths.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column28 = item.Background.CurrentAmortizationPeriod.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column29 = BooleanToCheckMark(item.Background.CurrentUsageStatus)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column30 = DblParser(item.Background.ChangeValueDecreaseAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column31 = DblParser(item.Background.ChangeValueDecreaseAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column32 = DblParser(item.Background.ChangeValueIncreaseAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column33 = DblParser(item.Background.ChangeValueIncreaseAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column34 = DblParser(item.Background.ChangeAssetUnitValue, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column35 = DblParser(item.Background.ChangeAssetValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column36 = DblParser(item.Background.ChangeAssetRevaluedPortionUnitValue, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column37 = DblParser(item.Background.ChangeAssetRevaluedPortionValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column38 = DblParser(item.Background.AfterOperationValueDecreaseAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column39 = DblParser(item.Background.AfterOperationValueDecreaseAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column40 = DblParser(item.Background.AfterOperationValueIncreaseAccountValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column41 = DblParser(item.Background.AfterOperationValueIncreaseAccountValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column42 = DblParser(item.Background.AfterOperationAssetValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column43 = DblParser(item.Background.AfterOperationAssetValuePerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column44 = DblParser(item.Background.AfterOperationAssetValueRevaluedPortion, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column45 = DblParser(item.Background.AfterOperationAssetValueRevaluedPortionPerUnit, ROUNDUNITASSET)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column46 = DblParser(item.ValueChangeTotal, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column47 = DblParser(item.ValueChangePerUnit, ROUNDUNITASSET)

        Next

        ReportFileName = "R_LongTermAssetValueChange.rdlc"
        NumberOfTablesInUse = 1

    End Sub

#End Region

#Region "Workers"

    ''' <summary>
    ''' Map <see cref="ActiveReports.Declaration">Declaration</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As ActiveReports.Declaration, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        CopyDataTable(RD.TableCompany, R_Obj.DeclarationDataSet, "General")
        CopyDataTable(RD.TableGeneral, R_Obj.DeclarationDataSet, "Specific")
        NumberOfTablesInUse = 0

        If R_Obj.Criteria.DeclarationType.DetailsTableCount > 0 Then

            CopyDataTable(RD.Table1, R_Obj.DeclarationDataSet, "Details")
            NumberOfTablesInUse = 1

        End If

        ReportFileName = R_Obj.Criteria.DeclarationType.RdlcFileName

    End Sub

    ''' <summary>
    ''' Map <see cref="ActiveReports.ImprestSheetInfoList">ImprestSheetInfoList</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As ActiveReports.ImprestSheetInfoList, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.DateFrom.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.DateTo.ToShortDateString
        If R_Obj.ShowPayedOut Then
            RD.TableGeneral.Item(0).Column3 = "X"
        Else
            RD.TableGeneral.Item(0).Column3 = ""
        End If
        RD.TableGeneral.Item(0).Column4 = R_Obj.TotalWorkersCount.ToString
        RD.TableGeneral.Item(0).Column5 = DblParser(R_Obj.TotalSum)
        RD.TableGeneral.Item(0).Column6 = DblParser(R_Obj.TotalSumPayedOut)

        For Each item As ActiveReports.ImprestSheetInfo In R_Obj.GetFilteredList
            RD.Table1.Rows.Add()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.Year.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.Month.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.Date.ToShortDateString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.Number.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = item.WorkersCount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = DblParser(item.TotalSum)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = DblParser(item.TotalSumPayedOut)
        Next

        ReportFileName = "R_ImprestSheetInfoList.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Workers.ImprestSheet">ImprestSheet</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Workers.ImprestSheet, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.Number.ToString
        RD.TableGeneral.Item(0).Column2 = R_Obj.Date.Year.ToString
        RD.TableGeneral.Item(0).Column3 = GetLithuanianMonth(R_Obj.Date.Month)
        RD.TableGeneral.Item(0).Column4 = R_Obj.Date.Day.ToString
        RD.TableGeneral.Item(0).Column5 = R_Obj.Year.ToString
        RD.TableGeneral.Item(0).Column6 = GetLithuanianMonth(R_Obj.Month)
        RD.TableGeneral.Item(0).Column7 = SumLT(R_Obj.TotalSum, 0, True, GetCurrentCompany.BaseCurrency)
        RD.TableGeneral.Item(0).Column8 = Convert.ToInt32(Math.Floor(R_Obj.TotalSum)).ToString
        RD.TableGeneral.Item(0).Column9 = DblParser((R_Obj.TotalSum - Math.Floor(R_Obj.TotalSum)) * 100, 0)

        For Each item As Workers.ImprestItem In R_Obj.ItemsSorted
            RD.Table1.Rows.Add()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.PersonName
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.PersonCode
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.ContractSerial & item.ContractNumber
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = DblParser(item.PayOffSumTotal)
        Next

        ReportFileName = "R_ImprestSheet.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="ActiveReports.PayOutNaturalPersonInfoList">PayOutNaturalPersonInfoList</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As ActiveReports.PayOutNaturalPersonInfoList, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.DateFrom.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.DateTo.ToShortDateString

        For Each item As ActiveReports.PayOutNaturalPersonInfo In R_Obj.GetSortedList
            RD.Table1.Rows.Add()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.Date.ToShortDateString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.DocNumber
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.Content
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.PersonInfo.Trim
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = item.PersonCodeSODRA.Trim
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = DblParser(item.SumBruto)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = DblParser(item.RateGPM)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = DblParser(item.DeductionGPM)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = DblParser(item.RatePSDForPerson)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = DblParser(item.DeductionPSD)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = DblParser(item.RateSODRAForPerson)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(item.DeductionSODRA)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = DblParser(item.SumNeto)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(item.RatePSDForCompany)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = DblParser(item.ContributionPSD)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = DblParser(item.RateSODRAForCompany)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = DblParser(item.ContributionSODRA)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = GetMinLengthString(item.CodeVMI, 2, "0"c)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = GetMinLengthString(item.CodeSODRA, 2, "0"c)
        Next

        ReportFileName = "R_PayOutNaturalPersonList.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="ActiveReports.WageSheetInfoList">WageSheetInfoList</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As ActiveReports.WageSheetInfoList, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.DateFrom.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.DateTo.ToShortDateString
        If R_Obj.ShowPayedOut Then
            RD.TableGeneral.Item(0).Column3 = "Taip"
        Else
            RD.TableGeneral.Item(0).Column3 = "Ne"
        End If

        For Each item As ActiveReports.WageSheetInfo In R_Obj.GetFilteredList
            RD.Table1.Rows.Add()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.Year.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.Month.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.Date.ToShortDateString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.Number.ToString
            If item.IsNonClosing Then
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = "X"
            Else
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = ""
            End If
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = item.WorkersCount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = DblParser(item.HoursWorked, 3)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = item.DaysWorked.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = DblParser(item.PayOutWage + item.PayOutSickLeave)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = DblParser(item.PayOutHoliday)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = DblParser(item.PayOutRedundancy)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(item.DeductionsGPM)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = DblParser(item.DeductionsSODRA + item.DeductionsPSD _
                + item.DeductionsPSDSickLeave)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(item.DeductionsOther)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = DblParser(item.DeductionsImprest)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = DblParser(item.PayOutAfterDeductions)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = DblParser(item.PayedOut)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = DblParser(item.Debt)
        Next

        ReportFileName = "R_WageSheetInfoList.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="ActiveReports.WorkerWageInfoReport">WorkerWageInfoReport</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As ActiveReports.WorkerWageInfoReport, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.DateFrom.ToString("yyyy-MM-dd")
        RD.TableGeneral.Item(0).Column2 = R_Obj.DateTo.ToString("yyyy-MM-dd")
        RD.TableGeneral.Item(0).Column3 = R_Obj.ContractNumber.ToString
        RD.TableGeneral.Item(0).Column4 = R_Obj.ContractSerial
        If R_Obj.IsConsolidated Then
            RD.TableGeneral.Item(0).Column5 = "Taip"
        Else
            RD.TableGeneral.Item(0).Column5 = "Ne"
        End If
        RD.TableGeneral.Item(0).Column6 = R_Obj.PersonID.ToString
        RD.TableGeneral.Item(0).Column7 = R_Obj.PersonInfo
        RD.TableGeneral.Item(0).Column8 = DblParser(R_Obj.DebtAtTheStart, 2)
        RD.TableGeneral.Item(0).Column9 = DblParser(R_Obj.DebtAtEnd, 2)
        RD.TableGeneral.Item(0).Column10 = DblParser(R_Obj.UnusedHolidaysAtStart, ROUNDACCUMULATEDHOLIDAY)
        RD.TableGeneral.Item(0).Column11 = DblParser(R_Obj.UnusedHolidaysAtEnd, ROUNDACCUMULATEDHOLIDAY)

        Dim debt As Double = R_Obj.DebtAtTheStart
        Dim normalHoursWorked As Double = 0
        Dim hrHoursWorked As Double = 0
        Dim onHoursWorked As Double = 0
        Dim scHoursWorked As Double = 0
        Dim totalHoursWorked As Double = 0
        Dim truancyHours As Double = 0
        Dim totalDaysWorked As Integer = 0
        Dim holidayWD As Integer = 0
        Dim holidayRD As Integer = 0
        Dim sickDays As Integer = 0
        Dim standartHours As Double = 0
        Dim standartDays As Integer = 0
        Dim bonusPayQuarterly As Double = 0
        Dim bonusPayAnnual As Double = 0
        Dim otherPayRelatedToWork As Double = 0
        Dim otherPayNotRelatedToWork As Double = 0
        Dim payOutWage As Double = 0
        Dim payOutExtraPay As Double = 0
        Dim payOutHR As Double = 0
        Dim payOutON As Double = 0
        Dim payOutSC As Double = 0
        Dim payOutSickLeave As Double = 0
        Dim payOutHoliday As Double = 0
        Dim payOutTotal As Double = 0
        Dim deductionGPM As Double = 0
        Dim deductionPSD As Double = 0
        Dim deductionSODRA As Double = 0
        Dim deductionOther As Double = 0
        Dim contributionSODRA As Double = 0
        Dim contributionPSD As Double = 0
        Dim contributionGuaranteeFund As Double = 0
        Dim payableTotal As Double = 0
        Dim npd As Double = 0
        Dim pnpd As Double = 0
        Dim payedOutTotalSum As Double = 0
        Dim totalDeductions As Double = 0
        Dim totalContributions As Double = 0

        For Each item As ActiveReports.WorkerWageInfo In R_Obj.Items

            RD.Table1.Rows.Add()

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.Year.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.Month.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = DblParser(item.RateSODRAEmployee, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = DblParser(item.RateSODRAEmployer, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = DblParser(item.RatePSDEmployee, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = DblParser(item.RatePSDEmployer, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = DblParser(item.RateGuaranteeFund, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = DblParser(item.RateGPM, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = DblParser(item.RateHR, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = DblParser(item.RateSC, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = DblParser(item.RateON, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(item.RateSickLeave, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = item.NPDFormula
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(item.WorkLoad, ROUNDWORKLOAD)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = DblParser(item.ApplicableVDUHourly, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = DblParser(item.ApplicableVDUDaily, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = DblParser(item.NormalHoursWorked, ROUNDWORKHOURS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = DblParser(item.HRHoursWorked, ROUNDWORKHOURS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = DblParser(item.ONHoursWorked, ROUNDWORKHOURS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column20 = DblParser(item.SCHoursWorked, ROUNDWORKHOURS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column21 = DblParser(item.TotalHoursWorked, ROUNDWORKHOURS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column22 = DblParser(item.TruancyHours, ROUNDWORKHOURS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column23 = item.TotalDaysWorked.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column24 = item.HolidayWD.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column25 = item.HolidayRD.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column26 = item.SickDays.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column27 = DblParser(item.StandartHours, ROUNDWORKHOURS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column28 = item.StandartDays.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column29 = DblParser(item.ConventionalWage, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column30 = item.WageTypeHumanReadable
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column31 = DblParser(item.ConventionalExtraPay, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column32 = DblParser(item.BonusPayQuarterly, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column33 = DblParser(item.BonusPayAnnual, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column34 = DblParser(item.OtherPayRelatedToWork, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column35 = DblParser(item.OtherPayNotRelatedToWork, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column36 = DblParser(item.PayOutWage, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column37 = DblParser(item.PayOutExtraPay, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column38 = DblParser(item.PayOutHR, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column39 = DblParser(item.PayOutON, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column40 = DblParser(item.PayOutSC, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column41 = DblParser(item.PayOutSickLeave, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column42 = DblParser(item.UnusedHolidayDaysForCompensation, ROUNDACCUMULATEDHOLIDAY)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column43 = DblParser(item.PayOutHoliday, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column44 = DblParser(item.PayOutUnusedHolidayCompensation, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column45 = DblParser(item.PayOutRedundancyPay, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column46 = DblParser(item.PayOutTotal, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column47 = DblParser(item.DeductionGPM, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column48 = DblParser(item.DeductionPSD + item.DeductionPSDSickLeave, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column49 = DblParser(item.DeductionPSDSickLeave, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column50 = DblParser(item.DeductionSODRA, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column51 = DblParser(item.DeductionOther, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column52 = DblParser(item.ContributionSODRA, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column53 = DblParser(item.ContributionPSD, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column54 = DblParser(item.ContributionGuaranteeFund, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column55 = DblParser(item.PayableTotal, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column56 = DblParser(item.NPD, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column57 = DblParser(item.PNPD, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column58 = DblParser(item.HoursForVDU, ROUNDWORKHOURS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column59 = item.DaysForVDU.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column60 = DblParser(item.WageForVDU, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column61 = DblParser(item.PayedOutTotalSum, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column62 = DblParser(item.DeductionGPM _
                + item.DeductionPSD + item.DeductionPSDSickLeave + item.DeductionSODRA _
                + item.DeductionOther, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column63 = DblParser(item.ContributionSODRA _
                + item.ContributionPSD + item.ContributionGuaranteeFund, 2)

            debt = CRound(debt + item.PayOutTotal - item.PayedOutTotalSum, 2)

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column64 = DblParser(debt, 2)

            normalHoursWorked = CRound(normalHoursWorked + item.NormalHoursWorked, ROUNDWORKHOURS)
            hrHoursWorked = CRound(hrHoursWorked + item.HRHoursWorked, ROUNDWORKHOURS)
            onHoursWorked = CRound(onHoursWorked + item.ONHoursWorked, ROUNDWORKHOURS)
            scHoursWorked = CRound(scHoursWorked + item.SCHoursWorked, ROUNDWORKHOURS)
            totalHoursWorked = CRound(totalHoursWorked + item.TotalHoursWorked, ROUNDWORKHOURS)
            truancyHours = CRound(truancyHours + item.TruancyHours, ROUNDWORKHOURS)
            totalDaysWorked = totalDaysWorked + item.TotalDaysWorked
            holidayWD = holidayWD + item.HolidayWD
            holidayRD = holidayRD + item.HolidayRD
            sickDays = sickDays + item.SickDays
            standartHours = CRound(standartHours + item.StandartHours, ROUNDWORKHOURS)
            standartDays = standartDays + item.StandartDays
            bonusPayQuarterly = CRound(bonusPayQuarterly + item.BonusPayQuarterly, 2)
            bonusPayAnnual = CRound(bonusPayAnnual + item.BonusPayAnnual, 2)
            otherPayRelatedToWork = CRound(otherPayRelatedToWork + item.OtherPayRelatedToWork, 2)
            otherPayNotRelatedToWork = CRound(otherPayNotRelatedToWork + item.OtherPayNotRelatedToWork, 2)
            payOutWage = CRound(payOutWage + item.PayOutWage, 2)
            payOutExtraPay = CRound(payOutExtraPay + item.PayOutExtraPay, 2)
            payOutHR = CRound(payOutHR + item.PayOutHR, 2)
            payOutON = CRound(payOutON + item.PayOutON, 2)
            payOutSC = CRound(payOutSC + item.PayOutSC, 2)
            payOutSickLeave = CRound(payOutSickLeave + item.PayOutSickLeave, 2)
            payOutHoliday = CRound(payOutHoliday + item.PayOutHoliday, 2)
            payOutTotal = CRound(payOutTotal + item.PayOutTotal, 2)
            deductionGPM = CRound(deductionGPM + item.DeductionGPM, 2)
            deductionPSD = CRound(deductionPSD + item.DeductionPSD, 2)
            deductionSODRA = CRound(deductionSODRA + item.DeductionSODRA, 2)
            deductionOther = CRound(deductionOther + item.DeductionOther, 2)
            contributionSODRA = CRound(contributionSODRA + item.ContributionSODRA, 2)
            contributionPSD = CRound(contributionPSD + item.ContributionPSD, 2)
            contributionGuaranteeFund = CRound(contributionGuaranteeFund + item.ContributionGuaranteeFund, 2)
            payableTotal = CRound(payableTotal + item.PayableTotal, 2)
            npd = CRound(npd + item.NPD, 2)
            pnpd = CRound(pnpd + item.PNPD, 2)
            payedOutTotalSum = CRound(payedOutTotalSum + item.PayedOutTotalSum, 2)
            totalDeductions = CRound(totalDeductions + item.DeductionGPM _
                + item.DeductionPSD + item.DeductionPSDSickLeave + item.DeductionSODRA _
                + item.DeductionOther, 2)
            totalContributions = CRound(totalContributions + item.ContributionSODRA _
                + item.ContributionPSD + item.ContributionGuaranteeFund, 2)

        Next

        RD.TableGeneral.Item(0).Column12 = DblParser(normalHoursWorked, ROUNDWORKHOURS)
        RD.TableGeneral.Item(0).Column13 = DblParser(hrHoursWorked, ROUNDWORKHOURS)
        RD.TableGeneral.Item(0).Column14 = DblParser(onHoursWorked, ROUNDWORKHOURS)
        RD.TableGeneral.Item(0).Column15 = DblParser(scHoursWorked, ROUNDWORKHOURS)
        RD.TableGeneral.Item(0).Column16 = DblParser(totalHoursWorked, ROUNDWORKHOURS)
        RD.TableGeneral.Item(0).Column17 = DblParser(truancyHours, ROUNDWORKHOURS)
        RD.TableGeneral.Item(0).Column18 = totalDaysWorked.ToString()
        RD.TableGeneral.Item(0).Column19 = holidayWD.ToString()
        RD.TableGeneral.Item(0).Column20 = holidayRD.ToString()
        RD.TableGeneral.Item(0).Column21 = sickDays.ToString()
        RD.TableGeneral.Item(0).Column22 = DblParser(standartHours, ROUNDWORKHOURS)
        RD.TableGeneral.Item(0).Column23 = standartDays.ToString()
        RD.TableGeneral.Item(0).Column24 = DblParser(bonusPayQuarterly, 2)
        RD.TableGeneral.Item(0).Column25 = DblParser(bonusPayAnnual, 2)
        RD.TableGeneral.Item(0).Column26 = DblParser(otherPayRelatedToWork, 2)
        RD.TableGeneral.Item(0).Column27 = DblParser(otherPayNotRelatedToWork, 2)
        RD.TableGeneral.Item(0).Column28 = DblParser(payOutWage, 2)
        RD.TableGeneral.Item(0).Column29 = DblParser(payOutExtraPay, 2)
        RD.TableGeneral.Item(0).Column30 = DblParser(payOutHR, 2)
        RD.TableGeneral.Item(0).Column31 = DblParser(payOutON, 2)
        RD.TableGeneral.Item(0).Column32 = DblParser(payOutSC, 2)
        RD.TableGeneral.Item(0).Column33 = DblParser(payOutSickLeave, 2)
        RD.TableGeneral.Item(0).Column34 = DblParser(payOutHoliday, 2)
        RD.TableGeneral.Item(0).Column35 = DblParser(payOutTotal, 2)
        RD.TableGeneral.Item(0).Column36 = DblParser(deductionGPM, 2)
        RD.TableGeneral.Item(0).Column37 = DblParser(deductionPSD, 2)
        RD.TableGeneral.Item(0).Column38 = DblParser(deductionSODRA, 2)
        RD.TableGeneral.Item(0).Column39 = DblParser(deductionOther, 2)
        RD.TableGeneral.Item(0).Column40 = DblParser(contributionSODRA, 2)
        RD.TableGeneral.Item(0).Column41 = DblParser(contributionPSD, 2)
        RD.TableGeneral.Item(0).Column42 = DblParser(contributionGuaranteeFund, 2)
        RD.TableGeneral.Item(0).Column43 = DblParser(payableTotal, 2)
        RD.TableGeneral.Item(0).Column44 = DblParser(npd, 2)
        RD.TableGeneral.Item(0).Column45 = DblParser(pnpd, 2)
        RD.TableGeneral.Item(0).Column46 = DblParser(payedOutTotalSum, 2)
        RD.TableGeneral.Item(0).Column47 = DblParser(totalDeductions, 2)
        RD.TableGeneral.Item(0).Column48 = DblParser(totalContributions, 2)

        ReportFileName = "R_WorkerWageInfoReport.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Workers.WageSheet">WageSheet</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Workers.WageSheet, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.Date.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.Date.Year.ToString
        RD.TableGeneral.Item(0).Column3 = GetLithuanianMonth(R_Obj.Date.Month)
        RD.TableGeneral.Item(0).Column4 = R_Obj.Date.Day.ToString
        RD.TableGeneral.Item(0).Column5 = R_Obj.Year.ToString
        RD.TableGeneral.Item(0).Column6 = GetLithuanianMonth(R_Obj.Month)
        RD.TableGeneral.Item(0).Column7 = R_Obj.Number.ToString
        RD.TableGeneral.Item(0).Column8 = DblParser(R_Obj.TotalSum)
        RD.TableGeneral.Item(0).Column9 = DblParser(R_Obj.TotalSumAfterDeductions)
        RD.TableGeneral.Item(0).Column10 = SumLT(R_Obj.TotalSumAfterDeductions, 0, _
            True, GetCurrentCompany.BaseCurrency)
        RD.TableGeneral.Item(0).Column11 = Convert.ToInt32(Math.Floor(R_Obj.TotalSumAfterDeductions)).ToString
        RD.TableGeneral.Item(0).Column12 = DblParser((R_Obj.TotalSumAfterDeductions - _
            Math.Floor(R_Obj.TotalSumAfterDeductions)) * 100, 0)
        RD.TableGeneral.Item(0).Column13 = R_Obj.CostAccount.ToString
        If R_Obj.IsNonClosing Then
            RD.TableGeneral.Item(0).Column15 = "Taip"
        Else
            RD.TableGeneral.Item(0).Column15 = "Ne"
        End If
        RD.TableGeneral.Item(0).Column16 = R_Obj.Remarks
        If Not R_Obj.WageRates Is Nothing Then
            RD.TableGeneral.Item(0).Column17 = DblParser(R_Obj.WageRates.RateHR)
            RD.TableGeneral.Item(0).Column18 = DblParser(R_Obj.WageRates.RateON)
            RD.TableGeneral.Item(0).Column19 = DblParser(R_Obj.WageRates.RateSC)
            RD.TableGeneral.Item(0).Column20 = DblParser(R_Obj.WageRates.RateSickLeave)
            RD.TableGeneral.Item(0).Column21 = DblParser(R_Obj.WageRates.RateGPM)
            RD.TableGeneral.Item(0).Column22 = DblParser(R_Obj.WageRates.RatePSDEmployee)
            RD.TableGeneral.Item(0).Column23 = DblParser(R_Obj.WageRates.RateSODRAEmployee)
            RD.TableGeneral.Item(0).Column24 = DblParser(R_Obj.WageRates.RatePSDEmployer)
            RD.TableGeneral.Item(0).Column25 = DblParser(R_Obj.WageRates.RateSODRAEmployer)
            RD.TableGeneral.Item(0).Column26 = DblParser(R_Obj.WageRates.RateGuaranteeFund)
            RD.TableGeneral.Item(0).Column27 = R_Obj.WageRates.NPDFormula.Trim
        Else
            RD.TableGeneral.Item(0).Column17 = "NaN"
            RD.TableGeneral.Item(0).Column18 = "NaN"
            RD.TableGeneral.Item(0).Column19 = "NaN"
            RD.TableGeneral.Item(0).Column20 = "NaN"
            RD.TableGeneral.Item(0).Column21 = "NaN"
            RD.TableGeneral.Item(0).Column22 = "NaN"
            RD.TableGeneral.Item(0).Column23 = "NaN"
            RD.TableGeneral.Item(0).Column24 = "NaN"
            RD.TableGeneral.Item(0).Column25 = "NaN"
            RD.TableGeneral.Item(0).Column26 = "NaN"
            RD.TableGeneral.Item(0).Column27 = ""
        End If

        For Each item As Workers.WageItem In R_Obj.Items
            If item.IsChecked Then
                RD.Table1.Rows.Add()
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.PersonName
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.PersonCode
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.PersonCodeSODRA
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.ContractSerial.Trim & _
                    item.ContractNumber.ToString
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = DblParser(item.WorkLoad, 3)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = DblParser(item.ConventionalWage)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = item.WageTypeHumanReadable
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = DblParser(item.ConventionalExtraPay)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = DblParser(item.ApplicableVDUDaily)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = DblParser(item.ApplicableVDUHourly)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = item.StandartDays.ToString
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(item.StandartHours, 3)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = item.TotalDaysWorked.ToString
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(item.NormalHoursWorked, 3)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = DblParser(item.HRHoursWorked, 3)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = DblParser(item.ONHoursWorked, 3)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = DblParser(item.SCHoursWorked, 3)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = DblParser(item.TotalHoursWorked, 3)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = DblParser(item.TruancyHours, 3)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column20 = item.SickDays.ToString
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column21 = item.HolidayWD.ToString
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column22 = item.HolidayRD.ToString
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column23 = _
                    DblParser(item.AnnualWorkingDaysRatio, 4)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column24 = _
                    DblParser(item.UnusedHolidayDaysForCompensation)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column25 = DblParser(item.PayOutWage)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column26 = DblParser(item.PayOutExtraPay)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column27 = DblParser(item.ActualHourlyPay)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column28 = DblParser(item.PayOutHR)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column29 = DblParser(item.PayOutON)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column30 = DblParser(item.PayOutSC)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column31 = DblParser(item.PayOutWage + _
                    item.PayOutHR + item.PayOutON + item.PayOutSC)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column32 = DblParser(item.BonusPay)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column33 = item.BonusType.ToString
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column34 = DblParser(item.OtherPayRelatedToWork)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column35 = DblParser(item.OtherPayNotRelatedToWork)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column36 = DblParser(item.OtherPayRelatedToWork + _
                    item.OtherPayNotRelatedToWork)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column37 = DblParser(item.PayOutRedundancyPay)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column38 = DblParser(item.PayOutHoliday)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column39 = DblParser(item.PayOutUnusedHolidayCompensation)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column40 = DblParser(item.PayOutHoliday + _
                    item.PayOutUnusedHolidayCompensation)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column41 = DblParser(item.PayOutSickLeave)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column42 = DblParser(item.PayOutTotal)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column43 = DblParser(item.NPD)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column44 = DblParser(item.PNPD)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column45 = DblParser(item.DeductionGPM)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column46 = DblParser(item.DeductionPSD)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column47 = DblParser(item.DeductionPSDSickLeave)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column48 = DblParser(item.DeductionPSD + _
                    item.DeductionPSDSickLeave)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column49 = DblParser(item.DeductionSODRA)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column50 = DblParser(item.PayOutTotalAfterTaxes)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column51 = DblParser(item.DeductionImprest)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column52 = DblParser(item.DeductionOtherApplicable)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column53 = DblParser(item.PayOutTotalAfterDeductions)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column54 = DblParser(item.ContributionSODRA)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column55 = DblParser(item.ContributionPSD)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column56 = DblParser(item.ContributionGuaranteeFund)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column57 = DblParser(item.DaysForVDU)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column58 = DblParser(item.HoursForVDU)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column59 = DblParser(item.WageForVDU)
                If item.UnusedHolidayDaysForCompensation > 0 Then
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column60 = "X"
                Else
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column60 = ""
                End If
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column61 = DblParser(item.DeductionPSD + _
                    item.DeductionPSDSickLeave + item.DeductionSODRA)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column62 = DblParser(item.ContributionSODRA + _
                    item.ContributionPSD)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column63 = DblParser(item.DeductionPSD + _
                    item.DeductionPSDSickLeave + item.DeductionSODRA + item.DeductionGPM + _
                    item.DeductionImprest + item.DeductionOtherApplicable)

            End If
        Next

        If Version = 0 Then
            ReportFileName = "R_WageSheet(1).rdlc"
        ElseIf Version = 1 Then
            ReportFileName = "R_WageSheet(2).rdlc"
        ElseIf Version = 2 Then
            ReportFileName = "R_PayChecks.rdlc"
        Else
            Throw New NotSupportedException("Klaida. Darbo užmokesčio žiniaraščio " & _
                "spausdinamos formos 3 versijos dar nenupiešiau.")
        End If
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="ActiveReports.WorkTimeSheetInfoList">WorkTimeSheetInfoList</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As ActiveReports.WorkTimeSheetInfoList, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.DateFrom.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.DateTo.ToShortDateString

        For Each item As ActiveReports.WorkTimeSheetInfo In R_Obj.GetSortedList

            RD.Table1.Rows.Add()

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.Date.ToShortDateString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.Year.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.Month.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.SubDivision
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = item.WorkersCount.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = item.TotalWorkDays.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = DblParser(item.TotalWorkTime, ROUNDWORKTIME)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = DblParser(item.NightWork, ROUNDWORKTIME)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = DblParser(item.OvertimeWork, ROUNDWORKTIME)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = DblParser(item.PublicHolidaysAndRestDayWork, ROUNDWORKTIME)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = DblParser(item.UnusualWork, ROUNDWORKTIME)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(item.Truancy, ROUNDWORKTIME)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = DblParser(item.DownTime, ROUNDWORKTIME)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = item.AnnualHolidays.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = item.OtherHolidays.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = item.SickDays.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = DblParser(item.OtherIncluded, ROUNDWORKTIME)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = DblParser(item.OtherExcluded, ROUNDWORKTIME)

        Next

        ReportFileName = "R_WorkTimeSheetInfoList.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Workers.Contract">Contract</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Workers.Contract, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.Date.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.Serial
        RD.TableGeneral.Item(0).Column3 = R_Obj.Number.ToString
        RD.TableGeneral.Item(0).Column4 = R_Obj.Position
        RD.TableGeneral.Item(0).Column5 = R_Obj.Content
        RD.TableGeneral.Item(0).Column6 = DblParser(R_Obj.WorkLoad, 3)
        RD.TableGeneral.Item(0).Column7 = R_Obj.HumanReadableWageType
        RD.TableGeneral.Item(0).Column8 = DblParser(R_Obj.Wage, 2)
        RD.TableGeneral.Item(0).Column9 = DblParser(R_Obj.ExtraPay, 2)
        RD.TableGeneral.Item(0).Column10 = R_Obj.AnnualHoliday.ToString
        RD.TableGeneral.Item(0).Column11 = DblParser(R_Obj.NPD)
        RD.TableGeneral.Item(0).Column12 = DblParser(R_Obj.PNPD)
        RD.TableGeneral.Item(0).Column13 = R_Obj.PersonName

        ReportFileName = "R_LabourContract.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Workers.ContractUpdate">ContractUpdate</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Workers.ContractUpdate, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.Date.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.Content
        RD.TableGeneral.Item(0).Column3 = R_Obj.Number.ToString
        RD.TableGeneral.Item(0).Column4 = R_Obj.Serial
        RD.TableGeneral.Item(0).Column5 = R_Obj.Content
        ' RD.TableGeneral.Item(0).Column6 = R_Obj.Value
        RD.TableGeneral.Item(0).Column7 = R_Obj.HumanReadableWageType
        RD.TableGeneral.Item(0).Column8 = R_Obj.PersonName
        RD.TableGeneral.Item(0).Column9 = R_Obj.HumanReadableWageType

        'If R_Obj.Type = Workers.WorkerStatusType.Employed Then
        '    Throw New NotImplementedException("Darbo sutarties spausdinimas neimplementuotas.")
        'ElseIf R_Obj.Type = Workers.WorkerStatusType.Fired Then
        '    Throw New NotImplementedException("Įsakymo dėl darbo sutarties nutraukimo spausdinimas neimplementuotas.")
        'ElseIf R_Obj.Type = Workers.WorkerStatusType.HolidayCorrection Then
        '    Throw New NotImplementedException("Atostogų korekcijos spausdinimas neimplementuotas.")
        'ElseIf R_Obj.Type = Workers.WorkerStatusType.Holiday Then
        '    ReportFileName = "R_LabourOrderOnHolidays.rdlc"
        'ElseIf R_Obj.Type = Workers.WorkerStatusType.ExtraPay _
        '    OrElse R_Obj.Type = Workers.WorkerStatusType.Wage _
        '    OrElse R_Obj.Type = Workers.WorkerStatusType.WorkLoad Then
        '    ReportFileName = "R_LabourContractAmendment.rdlc"
        'ElseIf R_Obj.Type = Workers.WorkerStatusType.NPD _
        '    OrElse R_Obj.Type = Workers.WorkerStatusType.PNPD Then
        '    ReportFileName = "R_MemorandumNPD.rdlc"
        'End If
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="ActiveReports.WorkersVDUInfo">WorkersVDUInfo</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As ActiveReports.WorkersVDUInfo, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.Date.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = BooleanToCheckMark(R_Obj.IncludeCurrentMonth)
        RD.TableGeneral.Item(0).Column3 = R_Obj.StandartDaysForTheCurrentMonth.ToString()
        RD.TableGeneral.Item(0).Column4 = DblParser(R_Obj.StandartHoursForTheCurrentMonth, ROUNDWORKHOURS)
        RD.TableGeneral.Item(0).Column5 = R_Obj.PersonID.ToString()
        RD.TableGeneral.Item(0).Column6 = R_Obj.PersonName
        RD.TableGeneral.Item(0).Column7 = R_Obj.PersonCode
        RD.TableGeneral.Item(0).Column8 = R_Obj.PersonCodeSODRA
        RD.TableGeneral.Item(0).Column9 = R_Obj.ContractSerial
        RD.TableGeneral.Item(0).Column10 = R_Obj.ContractNumber.ToString
        RD.TableGeneral.Item(0).Column11 = R_Obj.Position
        RD.TableGeneral.Item(0).Column12 = DblParser(R_Obj.WorkLoad, ROUNDWORKLOAD)
        RD.TableGeneral.Item(0).Column13 = DblParser(R_Obj.ConventionalWage, 2)
        RD.TableGeneral.Item(0).Column14 = R_Obj.WageTypeHumanReadable
        RD.TableGeneral.Item(0).Column15 = DblParser(R_Obj.ConventionalExtraPay, 2)
        RD.TableGeneral.Item(0).Column16 = DblParser(R_Obj.ApplicableVDUDaily, 2)
        RD.TableGeneral.Item(0).Column17 = DblParser(R_Obj.ApplicableVDUHourly, 2)
        RD.TableGeneral.Item(0).Column18 = DblParser(R_Obj.TotalWage, 2)
        RD.TableGeneral.Item(0).Column19 = R_Obj.TotalWorkDays.ToString()
        RD.TableGeneral.Item(0).Column20 = DblParser(R_Obj.TotalWorkHours, ROUNDWORKHOURS)
        RD.TableGeneral.Item(0).Column21 = DblParser(R_Obj.WageVDUDaily, 2)
        RD.TableGeneral.Item(0).Column22 = DblParser(R_Obj.WageVDUHourly, 2)
        RD.TableGeneral.Item(0).Column23 = DblParser(R_Obj.BonusYearly, 2)
        RD.TableGeneral.Item(0).Column24 = DblParser(R_Obj.BonusQuarterly, 2)
        RD.TableGeneral.Item(0).Column25 = DblParser(R_Obj.BonusBase, 2)
        RD.TableGeneral.Item(0).Column26 = R_Obj.TotalScheduledDays.ToString()
        RD.TableGeneral.Item(0).Column27 = DblParser(R_Obj.TotalScheduledHours, ROUNDWORKHOURS)
        RD.TableGeneral.Item(0).Column28 = DblParser(R_Obj.BonusVDUDaily, 2)
        RD.TableGeneral.Item(0).Column29 = DblParser(R_Obj.BonusVDUHourly, 2)

        For Each item As ActiveReports.WageVDUInfo In R_Obj.WageList

            RD.Table1.Rows.Add()

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.Year.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.Month.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.ScheduledDays.ToString()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = DblParser(item.ScheduledHours, ROUNDWORKHOURS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = item.WorkDays.ToString()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = DblParser(item.WorkHours, ROUNDWORKHOURS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = DblParser(item.Wage, 2)

        Next

        For Each item As ActiveReports.BonusVDUInfo In R_Obj.BonusList

            RD.Table2.Rows.Add()

            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column1 = item.Year.ToString
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column2 = item.Month.ToString
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column3 = DblParser(item.BonusAmount, 2)
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column4 = item.BonusTypeHumanReadable

        Next

        ReportFileName = "R_WorkersVDUInfo.rdlc"
        NumberOfTablesInUse = 2

    End Sub

    ''' <summary>
    ''' Map <see cref="ActiveReports.WorkerHolidayInfo">WorkerHolidayInfo</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As ActiveReports.WorkerHolidayInfo, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.Date.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = BooleanToCheckMark(R_Obj.IsForCompensation)
        RD.TableGeneral.Item(0).Column3 = R_Obj.PersonID.ToString()
        RD.TableGeneral.Item(0).Column4 = R_Obj.PersonName
        RD.TableGeneral.Item(0).Column5 = R_Obj.PersonCode
        RD.TableGeneral.Item(0).Column6 = R_Obj.PersonCodeSodra
        RD.TableGeneral.Item(0).Column7 = R_Obj.ContractDate.ToShortDateString()
        RD.TableGeneral.Item(0).Column8 = R_Obj.ContractSerial
        RD.TableGeneral.Item(0).Column9 = R_Obj.ContractNumber.ToString()
        RD.TableGeneral.Item(0).Column10 = BooleanToCheckMark(R_Obj.ContractIsTerminated)
        RD.TableGeneral.Item(0).Column11 = R_Obj.ContractTerminationDate
        RD.TableGeneral.Item(0).Column12 = BooleanToCheckMark(R_Obj.CompensationIsGranted)
        RD.TableGeneral.Item(0).Column13 = R_Obj.Position
        RD.TableGeneral.Item(0).Column14 = R_Obj.HolidayRate.ToString()
        RD.TableGeneral.Item(0).Column15 = DblParser(R_Obj.WorkLoad, ROUNDWORKLOAD)
        RD.TableGeneral.Item(0).Column16 = R_Obj.TotalWorkPeriodInDays.ToString()
        RD.TableGeneral.Item(0).Column17 = DblParser(R_Obj.TotalWorkPeriodInYears, ROUNDWORKYEARS)
        RD.TableGeneral.Item(0).Column18 = DblParser(R_Obj.TotalCumulatedHolidayDays, ROUNDACCUMULATEDHOLIDAY)
        RD.TableGeneral.Item(0).Column19 = R_Obj.TotalHolidayDaysGranted.ToString()
        RD.TableGeneral.Item(0).Column20 = DblParser(R_Obj.TotalHolidayDaysCompensated, ROUNDACCUMULATEDHOLIDAY)
        RD.TableGeneral.Item(0).Column21 = DblParser(R_Obj.TotalHolidayDaysCorrection, ROUNDACCUMULATEDHOLIDAY)
        RD.TableGeneral.Item(0).Column22 = DblParser(R_Obj.TotalHolidayDaysUsed, ROUNDACCUMULATEDHOLIDAY)
        RD.TableGeneral.Item(0).Column23 = DblParser(R_Obj.TotalUnusedHolidayDays, ROUNDACCUMULATEDHOLIDAY)

        For Each item As ActiveReports.HolidayCalculationPeriod In R_Obj.HolidayCalculatedList

            RD.Table1.Rows.Add()

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.DateBegin.ToShortDateString()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.DateEnd.ToShortDateString()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.LengthDays.ToString()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = DblParser(item.LengthYears, ROUNDWORKYEARS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = DblParser(item.CumulatedHolidayDaysPerPeriod, ROUNDACCUMULATEDHOLIDAY)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = item.HolidayRate.ToString()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = item.StatusDescription

        Next

        For Each item As ActiveReports.HolidaySpentItem In R_Obj.HolidaySpentList

            RD.Table2.Rows.Add()

            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column1 = item.TypeHumanReadable
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column2 = item.DocumentID.ToString()
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column3 = item.DocumentDate.ToShortDateString()
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column4 = item.DocumentNumber
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column5 = item.Spent.ToString()
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column6 = DblParser(item.Compensated, ROUNDACCUMULATEDHOLIDAY)
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column7 = DblParser(item.Correction, ROUNDACCUMULATEDHOLIDAY)
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column8 = DblParser(item.Total, ROUNDACCUMULATEDHOLIDAY)
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column9 = item.DocumentContent

        Next

        ReportFileName = "R_WorkerHolidayInfo.rdlc"
        NumberOfTablesInUse = 2

    End Sub

    ''' <summary>
    ''' Map <see cref="WageSheetItem">WageSheetItem</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As WageSheetItem, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.Sheet.Date.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.Sheet.Date.Year.ToString
        RD.TableGeneral.Item(0).Column3 = GetLithuanianMonth(R_Obj.Sheet.Date.Month)
        RD.TableGeneral.Item(0).Column4 = R_Obj.Sheet.Date.Day.ToString
        RD.TableGeneral.Item(0).Column5 = R_Obj.Sheet.Year.ToString
        RD.TableGeneral.Item(0).Column6 = GetLithuanianMonth(R_Obj.Sheet.Month)
        RD.TableGeneral.Item(0).Column7 = R_Obj.Sheet.Number.ToString
        RD.TableGeneral.Item(0).Column8 = DblParser(R_Obj.Sheet.TotalSum)
        RD.TableGeneral.Item(0).Column9 = DblParser(R_Obj.Sheet.TotalSumAfterDeductions)
        RD.TableGeneral.Item(0).Column10 = SumLT(R_Obj.Sheet.TotalSumAfterDeductions, 0, _
            True, GetCurrentCompany.BaseCurrency)
        RD.TableGeneral.Item(0).Column11 = Convert.ToInt32(Math.Floor(R_Obj.Sheet.TotalSumAfterDeductions)).ToString
        RD.TableGeneral.Item(0).Column12 = DblParser((R_Obj.Sheet.TotalSumAfterDeductions - _
            Math.Floor(R_Obj.Sheet.TotalSumAfterDeductions)) * 100, 0)
        RD.TableGeneral.Item(0).Column13 = R_Obj.Sheet.CostAccount.ToString
        If R_Obj.Sheet.IsNonClosing Then
            RD.TableGeneral.Item(0).Column15 = "Taip"
        Else
            RD.TableGeneral.Item(0).Column15 = "Ne"
        End If
        RD.TableGeneral.Item(0).Column16 = R_Obj.Sheet.Remarks
        If Not R_Obj.Sheet.WageRates Is Nothing Then
            RD.TableGeneral.Item(0).Column17 = DblParser(R_Obj.Sheet.WageRates.RateHR)
            RD.TableGeneral.Item(0).Column18 = DblParser(R_Obj.Sheet.WageRates.RateON)
            RD.TableGeneral.Item(0).Column19 = DblParser(R_Obj.Sheet.WageRates.RateSC)
            RD.TableGeneral.Item(0).Column20 = DblParser(R_Obj.Sheet.WageRates.RateSickLeave)
            RD.TableGeneral.Item(0).Column21 = DblParser(R_Obj.Sheet.WageRates.RateGPM)
            RD.TableGeneral.Item(0).Column22 = DblParser(R_Obj.Sheet.WageRates.RatePSDEmployee)
            RD.TableGeneral.Item(0).Column23 = DblParser(R_Obj.Sheet.WageRates.RateSODRAEmployee)
            RD.TableGeneral.Item(0).Column24 = DblParser(R_Obj.Sheet.WageRates.RatePSDEmployer)
            RD.TableGeneral.Item(0).Column25 = DblParser(R_Obj.Sheet.WageRates.RateSODRAEmployer)
            RD.TableGeneral.Item(0).Column26 = DblParser(R_Obj.Sheet.WageRates.RateGuaranteeFund)
            RD.TableGeneral.Item(0).Column27 = R_Obj.Sheet.WageRates.NPDFormula.Trim
        Else
            RD.TableGeneral.Item(0).Column17 = "NaN"
            RD.TableGeneral.Item(0).Column18 = "NaN"
            RD.TableGeneral.Item(0).Column19 = "NaN"
            RD.TableGeneral.Item(0).Column20 = "NaN"
            RD.TableGeneral.Item(0).Column21 = "NaN"
            RD.TableGeneral.Item(0).Column22 = "NaN"
            RD.TableGeneral.Item(0).Column23 = "NaN"
            RD.TableGeneral.Item(0).Column24 = "NaN"
            RD.TableGeneral.Item(0).Column25 = "NaN"
            RD.TableGeneral.Item(0).Column26 = "NaN"
            RD.TableGeneral.Item(0).Column27 = ""
        End If

        Dim item As Workers.WageItem = R_Obj.Item

        RD.Table1.Rows.Add()
        RD.Table1.Item(0).Column1 = item.PersonName
        RD.Table1.Item(0).Column2 = item.PersonCode
        RD.Table1.Item(0).Column3 = item.PersonCodeSODRA
        RD.Table1.Item(0).Column4 = item.ContractSerial.Trim & _
            item.ContractNumber.ToString
        RD.Table1.Item(0).Column5 = DblParser(item.WorkLoad, 3)
        RD.Table1.Item(0).Column6 = DblParser(item.ConventionalWage)
        RD.Table1.Item(0).Column7 = item.WageTypeHumanReadable
        RD.Table1.Item(0).Column8 = DblParser(item.ConventionalExtraPay)
        RD.Table1.Item(0).Column9 = DblParser(item.ApplicableVDUDaily)
        RD.Table1.Item(0).Column10 = DblParser(item.ApplicableVDUHourly)
        RD.Table1.Item(0).Column11 = item.StandartDays.ToString
        RD.Table1.Item(0).Column12 = DblParser(item.StandartHours, 3)
        RD.Table1.Item(0).Column13 = item.TotalDaysWorked.ToString
        RD.Table1.Item(0).Column14 = DblParser(item.NormalHoursWorked, 3)
        RD.Table1.Item(0).Column15 = DblParser(item.HRHoursWorked, 3)
        RD.Table1.Item(0).Column16 = DblParser(item.ONHoursWorked, 3)
        RD.Table1.Item(0).Column17 = DblParser(item.SCHoursWorked, 3)
        RD.Table1.Item(0).Column18 = DblParser(item.TotalHoursWorked, 3)
        RD.Table1.Item(0).Column19 = DblParser(item.TruancyHours, 3)
        RD.Table1.Item(0).Column20 = item.SickDays.ToString
        RD.Table1.Item(0).Column21 = item.HolidayWD.ToString
        RD.Table1.Item(0).Column22 = item.HolidayRD.ToString
        RD.Table1.Item(0).Column23 = _
            DblParser(item.AnnualWorkingDaysRatio, 4)
        RD.Table1.Item(0).Column24 = _
            DblParser(item.UnusedHolidayDaysForCompensation)
        RD.Table1.Item(0).Column25 = DblParser(item.PayOutWage)
        RD.Table1.Item(0).Column26 = DblParser(item.PayOutExtraPay)
        RD.Table1.Item(0).Column27 = DblParser(item.ActualHourlyPay)
        RD.Table1.Item(0).Column28 = DblParser(item.PayOutHR)
        RD.Table1.Item(0).Column29 = DblParser(item.PayOutON)
        RD.Table1.Item(0).Column30 = DblParser(item.PayOutSC)
        RD.Table1.Item(0).Column31 = DblParser(item.PayOutWage + _
            item.PayOutHR + item.PayOutON + item.PayOutSC)
        RD.Table1.Item(0).Column32 = DblParser(item.BonusPay)
        RD.Table1.Item(0).Column33 = item.BonusType.ToString
        RD.Table1.Item(0).Column34 = DblParser(item.OtherPayRelatedToWork)
        RD.Table1.Item(0).Column35 = DblParser(item.OtherPayNotRelatedToWork)
        RD.Table1.Item(0).Column36 = DblParser(item.OtherPayRelatedToWork + _
            item.OtherPayNotRelatedToWork)
        RD.Table1.Item(0).Column37 = DblParser(item.PayOutRedundancyPay)
        RD.Table1.Item(0).Column38 = DblParser(item.PayOutHoliday)
        RD.Table1.Item(0).Column39 = DblParser(item.PayOutUnusedHolidayCompensation)
        RD.Table1.Item(0).Column40 = DblParser(item.PayOutHoliday + _
            item.PayOutUnusedHolidayCompensation)
        RD.Table1.Item(0).Column41 = DblParser(item.PayOutSickLeave)
        RD.Table1.Item(0).Column42 = DblParser(item.PayOutTotal)
        RD.Table1.Item(0).Column43 = DblParser(item.NPD)
        RD.Table1.Item(0).Column44 = DblParser(item.PNPD)
        RD.Table1.Item(0).Column45 = DblParser(item.DeductionGPM)
        RD.Table1.Item(0).Column46 = DblParser(item.DeductionPSD)
        RD.Table1.Item(0).Column47 = DblParser(item.DeductionPSDSickLeave)
        RD.Table1.Item(0).Column48 = DblParser(item.DeductionPSD + _
            item.DeductionPSDSickLeave)
        RD.Table1.Item(0).Column49 = DblParser(item.DeductionSODRA)
        RD.Table1.Item(0).Column50 = DblParser(item.PayOutTotalAfterTaxes)
        RD.Table1.Item(0).Column51 = DblParser(item.DeductionImprest)
        RD.Table1.Item(0).Column52 = DblParser(item.DeductionOtherApplicable)
        RD.Table1.Item(0).Column53 = DblParser(item.PayOutTotalAfterDeductions)
        RD.Table1.Item(0).Column54 = DblParser(item.ContributionSODRA)
        RD.Table1.Item(0).Column55 = DblParser(item.ContributionPSD)
        RD.Table1.Item(0).Column56 = DblParser(item.ContributionGuaranteeFund)
        RD.Table1.Item(0).Column57 = DblParser(item.DaysForVDU)
        RD.Table1.Item(0).Column58 = DblParser(item.HoursForVDU)
        RD.Table1.Item(0).Column59 = DblParser(item.WageForVDU)
        If item.UnusedHolidayDaysForCompensation > 0 Then
            RD.Table1.Item(0).Column60 = "X"
        Else
            RD.Table1.Item(0).Column60 = ""
        End If
        RD.Table1.Item(0).Column61 = DblParser(item.DeductionPSD + _
            item.DeductionPSDSickLeave + item.DeductionSODRA)
        RD.Table1.Item(0).Column62 = DblParser(item.ContributionSODRA + _
            item.ContributionPSD)
        RD.Table1.Item(0).Column63 = DblParser(item.DeductionPSD + _
            item.DeductionPSDSickLeave + item.DeductionSODRA + item.DeductionGPM + _
            item.DeductionImprest + item.DeductionOtherApplicable)


        ReportFileName = "R_PayChecks.rdlc"

        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Workers.HolidayPayReserve">HolidayPayReserve</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Workers.HolidayPayReserve, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.ID.ToString()
        RD.TableGeneral.Item(0).Column2 = R_Obj.Date.ToShortDateString()
        RD.TableGeneral.Item(0).Column3 = R_Obj.Date.Year.ToString
        RD.TableGeneral.Item(0).Column4 = GetLithuanianMonth(R_Obj.Date.Month)
        RD.TableGeneral.Item(0).Column5 = R_Obj.Date.Day.ToString
        RD.TableGeneral.Item(0).Column6 = R_Obj.Number
        RD.TableGeneral.Item(0).Column7 = R_Obj.Content
        RD.TableGeneral.Item(0).Column9 = R_Obj.AccountCosts.ToString()
        RD.TableGeneral.Item(0).Column10 = R_Obj.AccountReserve.ToString()
        RD.TableGeneral.Item(0).Column11 = R_Obj.Comments
        RD.TableGeneral.Item(0).Column12 = R_Obj.InsertDate.ToString()
        RD.TableGeneral.Item(0).Column13 = R_Obj.UpdateDate.ToString()
        RD.TableGeneral.Item(0).Column14 = DblParser(R_Obj.TaxRate, 2)
        RD.TableGeneral.Item(0).Column15 = DblParser(R_Obj.TotalSumCurrent, 2)
        RD.TableGeneral.Item(0).Column16 = DblParser(R_Obj.TotalSumEvaluatedBeforeTaxes, 2)
        RD.TableGeneral.Item(0).Column17 = DblParser(R_Obj.TotalSumEvaluated, 2)
        RD.TableGeneral.Item(0).Column18 = DblParser(R_Obj.TotalSumChange, 2)

        For Each item As Workers.HolidayPayReserveItem In R_Obj.ItemsSorted

            RD.Table1.Rows.Add()

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.ID.ToString()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.PersonID.ToString()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.PersonName
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.PersonCode
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = item.PersonCodeSodra
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = item.ContractDate.ToShortDateString()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = item.ContractSerial
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = item.ContractNumber.ToString()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = item.ContractSerial & item.ContractNumber.ToString()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = item.Position
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = DblParser(item.WorkLoad, ROUNDWORKLOAD)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(item.ConventionalWage, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = item.WageTypeHumanReadable
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(item.ConventionalExtraPay, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = item.HolidayRate.ToString()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = item.TotalWorkPeriodInDays.ToString()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = DblParser(item.TotalWorkPeriodInYears, ROUNDWORKYEARS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = DblParser(item.TotalCumulatedHolidayDays, ROUNDACCUMULATEDHOLIDAY)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = item.TotalHolidayDaysGranted.ToString()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column20 = DblParser(item.TotalHolidayDaysCorrection, ROUNDACCUMULATEDHOLIDAY)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column21 = DblParser(item.TotalHolidayDaysUsed, ROUNDACCUMULATEDHOLIDAY)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column22 = DblParser(item.TotalUnusedHolidayDays, ROUNDACCUMULATEDHOLIDAY)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column23 = item.TotalScheduledDays.ToString()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column24 = DblParser(item.TotalScheduledHours, ROUNDWORKHOURS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column25 = item.StandartDaysForTheCurrentMonth.ToString()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column26 = DblParser(item.StandartHoursForTheCurrentMonth, ROUNDWORKHOURS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column27 = item.TotalWorkDays.ToString()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column28 = DblParser(item.TotalWorkHours, ROUNDWORKHOURS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column29 = DblParser(item.TotalWage, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column30 = DblParser(item.BonusQuarterly, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column31 = DblParser(item.BonusYearly, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column32 = DblParser(item.BonusBase, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column33 = DblParser(item.TotalVDUDaily, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column34 = DblParser(item.TotalVDUHourly, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column35 = DblParser(item.ApplicableUnusedHolidayDays, ROUNDACCUMULATEDHOLIDAY)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column36 = DblParser(item.ApplicableVDUDaily, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column37 = DblParser(item.ApplicableWorkDaysRatio, ROUNDWORKDAYSRATIO)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column38 = DblParser(item.HolidayPayReserveValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column39 = item.Comments

        Next

        ReportFileName = "R_HolidayPayReserve.rdlc"
        NumberOfTablesInUse = 1

    End Sub

#Region " WorkTimeSheet maping methods "

    ''' <summary>
    ''' Map <see cref="Workers.WorkTimeSheet">WorkTimeSheet</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Workers.WorkTimeSheet, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.Date.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.Year.ToString
        RD.TableGeneral.Item(0).Column3 = R_Obj.Month.ToString
        RD.TableGeneral.Item(0).Column4 = R_Obj.Number
        RD.TableGeneral.Item(0).Column5 = R_Obj.PreparedByName
        RD.TableGeneral.Item(0).Column6 = R_Obj.PreparedByPosition
        RD.TableGeneral.Item(0).Column7 = R_Obj.SignedByName
        RD.TableGeneral.Item(0).Column8 = R_Obj.SignedByPosition
        RD.TableGeneral.Item(0).Column9 = R_Obj.SubDivision
        RD.TableGeneral.Item(0).Column10 = R_Obj.GetTotalDays.ToString
        RD.TableGeneral.Item(0).Column11 = R_Obj.GetTotalHours.ToString
        RD.TableGeneral.Item(0).Column12 = R_Obj.GetTotalHoursByType(Workers.WorkTimeType.NightWork).ToString
        RD.TableGeneral.Item(0).Column13 = R_Obj.GetTotalHoursByType(Workers.WorkTimeType.OvertimeWork).ToString
        RD.TableGeneral.Item(0).Column14 = R_Obj.GetTotalHoursByType(Workers.WorkTimeType.UnusualWork).ToString
        RD.TableGeneral.Item(0).Column15 = R_Obj.GetTotalHoursByType(Workers.WorkTimeType.PublicHolidaysAndRestDayWork).ToString
        RD.TableGeneral.Item(0).Column16 = R_Obj.GetTotalAbsenceDays.ToString
        RD.TableGeneral.Item(0).Column17 = R_Obj.GetTotalAbsenceHours.ToString

        Dim maxDayCount As Integer = Date.DaysInMonth(R_Obj.Year, R_Obj.Month)
        Dim workerCount As Integer = 1
        For Each item As Workers.WorkTimeItem In R_Obj.GeneralItemListSorted

            If item.IsChecked Then

                RD.Table1.Rows.Add()

                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.Worker
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.WorkerPosition
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.ContractSerial
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.ContractNumber.ToString
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = DblParser(item.WorkerLoad, 3)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = item.QuotaDays.ToString
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = item.QuotaHours.ToString
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = GetDayString(item, 1, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = GetDayString(item, 2, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = GetDayString(item, 3, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = GetDayString(item, 4, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = GetDayString(item, 5, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = GetDayString(item, 6, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = GetDayString(item, 7, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = GetDayString(item, 8, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = GetDayString(item, 9, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = GetDayString(item, 10, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = GetDayString(item, 11, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = GetDayString(item, 12, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column20 = GetDayString(item, 13, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column21 = GetDayString(item, 14, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column22 = GetDayString(item, 15, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column23 = GetDayString(item, 16, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column24 = GetDayString(item, 17, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column25 = GetDayString(item, 18, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column26 = GetDayString(item, 19, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column27 = GetDayString(item, 20, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column28 = GetDayString(item, 21, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column29 = GetDayString(item, 22, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column30 = GetDayString(item, 23, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column31 = GetDayString(item, 24, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column32 = GetDayString(item, 25, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column33 = GetDayString(item, 26, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column34 = GetDayString(item, 27, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column35 = GetDayString(item, 28, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column36 = GetDayString(item, 29, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column37 = GetDayString(item, 30, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column38 = GetDayString(item, 31, maxDayCount)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column39 = item.TotalDays.ToString
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column40 = item.TotalHours.ToString

                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column41 = ""
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column42 = ""
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column43 = ""
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column44 = ""
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column45 = ""
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column46 = ""
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column47 = ""

                GetAgregateTimeForItem(item, R_Obj, RD.Table1.Item(RD.Table1.Rows.Count - 1).Column41, _
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column42, _
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column43, _
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column44, _
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column45, _
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column46, _
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column47)

                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column48 = workerCount.ToString
                workerCount += 1

                For Each row As DataRow In GetDetailsDatatable(item, R_Obj).Rows

                    RD.Table1.Rows.Add()

                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = row(0).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = row(1).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = row(2).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = row(3).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = row(4).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = row(5).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = row(6).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = row(7).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = row(8).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = row(9).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = row(10).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = row(11).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column20 = row(12).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column21 = row(13).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column22 = row(14).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column23 = row(15).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column24 = row(16).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column25 = row(17).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column26 = row(18).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column27 = row(19).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column28 = row(20).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column29 = row(21).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column30 = row(22).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column31 = row(23).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column32 = row(24).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column33 = row(25).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column34 = row(26).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column35 = row(27).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column36 = row(28).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column37 = row(29).ToString
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column38 = row(30).ToString

                Next

            End If

        Next

        RD.TableGeneral.Item(0).Column18 = GetAggregateTable(R_Obj, RD).ToString

        ReportFileName = "R_WorkTimeSheet.rdlc"
        NumberOfTablesInUse = 2

    End Sub

    Private Function GetDayString(ByVal item As Workers.WorkTimeItem, _
        ByVal Day As Integer, ByVal maxDayCount As Integer) As String

        If Day < 1 Then Day = 1
        If Day > maxDayCount Then Return ""

        Try
            Dim Hours As Double = DirectCast(GetType(Workers.WorkTimeItem). _
                GetProperty("Day" & Day.ToString).GetValue(item, Nothing), Double)
            Dim itemType As HelperLists.WorkTimeClassInfo = DirectCast(GetType(Workers.WorkTimeItem). _
                GetProperty("DayType" & Day.ToString).GetValue(item, Nothing), HelperLists.WorkTimeClassInfo)
            If Not itemType Is Nothing AndAlso itemType.ID > 0 Then Return itemType.Code
            Return Hours.ToString
        Catch ex As Exception
            Return ""
        End Try

    End Function

    Private Function GetDetailsDatatable(ByVal item As Workers.WorkTimeItem, _
        ByVal sheet As Workers.WorkTimeSheet) As DataTable

        Dim result As New DataTable
        For i As Integer = 1 To 31
            result.Columns.Add()
        Next

        Dim maxDayCount As Integer = Date.DaysInMonth(sheet.Year, sheet.Month)

        For Each subitem As Workers.SpecialWorkTimeItem In sheet.SpecialItemList
            If subitem.WorkerID = item.WorkerID AndAlso subitem.ContractNumber = item.ContractNumber _
                AndAlso subitem.ContractSerial.Trim.ToUpper = item.ContractSerial.Trim.ToUpper Then

                result.Rows.Add(result.NewRow)

                For i As Integer = 1 To 31
                    result.Rows(result.Rows.Count - 1).Item(i - 1) = _
                        GetSpecialDayString(subitem, i, maxDayCount)
                Next

            End If
        Next

        For i As Integer = result.Rows.Count To 2 Step -1
            If MergeSpecialWorkTimeDataRows(result.Rows(i - 1), result.Rows(i - 2)) Then _
                result.Rows.RemoveAt(i - 1)
        Next

        Return result

    End Function

    Private Function GetSpecialDayString(ByVal item As Workers.SpecialWorkTimeItem, _
        ByVal Day As Integer, ByVal maxDayCount As Integer) As String

        If Day < 1 Then Day = 1
        If Day > maxDayCount Then Return ""

        Try
            Dim Hours As Double = DirectCast(GetType(Workers.SpecialWorkTimeItem). _
                GetProperty("Day" & Day.ToString).GetValue(item, Nothing), Double)
            If Not CRound(Hours, ROUNDWORKTIME) > 0 Then Return ""
            Dim TimeClass As String = ""
            If Not item.Type Is Nothing AndAlso item.Type.ID > 0 Then TimeClass = item.Type.Code
            Return TimeClass & Hours.ToString
        Catch ex As Exception
            Return ""
        End Try

    End Function

    Private Function MergeSpecialWorkTimeDataRows(ByVal source As DataRow, _
        ByRef target As DataRow) As Boolean

        For i As Integer = 1 To 31
            If Not String.IsNullOrEmpty(source(i - 1).ToString.Trim) AndAlso _
                Not String.IsNullOrEmpty(target(i - 1).ToString.Trim) Then Return False
        Next

        For i As Integer = 1 To 31
            If Not String.IsNullOrEmpty(source(i - 1).ToString.Trim) Then _
                target(i - 1) = source(i - 1).ToString.Trim
        Next

        Return True

    End Function

    Private Sub GetAgregateTimeForItem(ByVal item As Workers.WorkTimeItem, _
        ByVal sheet As Workers.WorkTimeSheet, ByRef NightTime As String, _
        ByRef Overtime As String, ByRef PublicHolidaysAndRestTime As String, _
        ByRef UnusualTime As String, ByRef AbsenceCode As String, _
        ByRef AbsenceDays As String, ByRef AbsenceHours As String)

        Dim cNightTime As Double = 0
        Dim cOvertime As Double = 0
        Dim cPublicHolidaysAndRestTime As Double = 0
        Dim cUnusualTime As Double = 0
        Dim cAbsenceCode As New List(Of String)
        Dim cAbsenceDays As Integer = 0
        Dim cAbsenceHours As Double = 0

        For Each subitem As Workers.SpecialWorkTimeItem In sheet.SpecialItemList
            If subitem.WorkerID = item.WorkerID AndAlso subitem.ContractNumber = item.ContractNumber _
                AndAlso subitem.ContractSerial.Trim.ToUpper = item.ContractSerial.Trim.ToUpper Then

                If Not subitem.Type Is Nothing AndAlso subitem.Type.ID > 0 Then

                    If subitem.Type.Type = Workers.WorkTimeType.AnnualHolidays OrElse _
                        subitem.Type.Type = Workers.WorkTimeType.DownTime OrElse _
                        subitem.Type.Type = Workers.WorkTimeType.OtherExcluded OrElse _
                        subitem.Type.Type = Workers.WorkTimeType.OtherHolidays OrElse _
                        subitem.Type.Type = Workers.WorkTimeType.SickDays OrElse _
                        subitem.Type.Type = Workers.WorkTimeType.Truancy Then

                        If Not cAbsenceCode.Contains(subitem.Type.Code.Trim.ToUpper) Then _
                            cAbsenceCode.Add(subitem.Type.Code.Trim.ToUpper)

                        If subitem.Type.WithoutWorkHours Then
                            cAbsenceDays += 1
                        Else
                            cAbsenceHours = CRound(cAbsenceHours + subitem.TotalHours, ROUNDWORKTIME)
                        End If

                    ElseIf subitem.Type.Type = Workers.WorkTimeType.NightWork Then

                        cNightTime = CRound(cNightTime + subitem.TotalHours, ROUNDWORKTIME)

                    ElseIf subitem.Type.Type = Workers.WorkTimeType.OvertimeWork Then

                        cOvertime = CRound(cOvertime + subitem.TotalHours, ROUNDWORKTIME)

                    ElseIf subitem.Type.Type = Workers.WorkTimeType.PublicHolidaysAndRestDayWork Then

                        cPublicHolidaysAndRestTime = CRound(cPublicHolidaysAndRestTime _
                            + subitem.TotalHours, ROUNDWORKTIME)

                    ElseIf subitem.Type.Type = Workers.WorkTimeType.UnusualWork Then

                        cUnusualTime = CRound(cUnusualTime + subitem.TotalHours, ROUNDWORKTIME)

                    End If


                End If

            End If

        Next

        Dim curType As HelperLists.WorkTimeClassInfo

        For i As Integer = 1 To 31

            curType = Nothing

            Try
                curType = DirectCast(GetType(Workers.WorkTimeItem).GetProperty( _
                    "DayType" & i.ToString).GetValue(item, Nothing), HelperLists.WorkTimeClassInfo)
            Catch ex As Exception
            End Try

            If Not curType Is Nothing AndAlso curType.ID > 0 AndAlso _
                curType.ID <> sheet.DefaultRestTimeClass.ID AndAlso _
                curType.ID <> sheet.DefaultPublicHolidayTimeClass.ID AndAlso _
                (curType.Type = Workers.WorkTimeType.AnnualHolidays OrElse _
                curType.Type = Workers.WorkTimeType.DownTime OrElse _
                curType.Type = Workers.WorkTimeType.OtherExcluded OrElse _
                curType.Type = Workers.WorkTimeType.OtherHolidays OrElse _
                curType.Type = Workers.WorkTimeType.SickDays OrElse _
                curType.Type = Workers.WorkTimeType.Truancy) Then

                If Not cAbsenceCode.Contains(curType.Code.Trim.ToUpper) Then _
                    cAbsenceCode.Add(curType.Code.Trim.ToUpper)
                cAbsenceDays += 1

            End If

        Next

        NightTime = cNightTime.ToString
        Overtime = cOvertime.ToString
        PublicHolidaysAndRestTime = cPublicHolidaysAndRestTime.ToString
        UnusualTime = cUnusualTime.ToString
        AbsenceCode = String.Join(",", cAbsenceCode.ToArray)
        AbsenceDays = cAbsenceDays.ToString
        AbsenceHours = cAbsenceHours.ToString

    End Sub

    Private Function GetAggregateTable(ByVal sheet As Workers.WorkTimeSheet, _
        ByRef RD As AccControls.ReportData) As Integer

        Dim resultDays As New Dictionary(Of String, Integer)
        Dim resultHours As New Dictionary(Of String, Double)

        For Each subitem As Workers.SpecialWorkTimeItem In sheet.SpecialItemList
            If Not subitem.Type Is Nothing AndAlso subitem.Type.ID > 0 Then
                If subitem.Type.WithoutWorkHours Then
                    If resultDays.ContainsKey(subitem.Type.Code.Trim.ToUpper) Then
                        resultDays(subitem.Type.Code.Trim.ToUpper) = _
                            resultDays(subitem.Type.Code.Trim.ToUpper) + 1
                    Else
                        resultDays.Add(subitem.Type.Code.Trim.ToUpper, 1)
                    End If
                Else
                    If resultHours.ContainsKey(subitem.Type.Code.Trim.ToUpper) Then
                        resultHours(subitem.Type.Code.Trim.ToUpper) = _
                            CRound(resultHours(subitem.Type.Code.Trim.ToUpper) _
                            + subitem.TotalHours, ROUNDWORKTIME)
                    Else
                        resultHours.Add(subitem.Type.Code.Trim.ToUpper, subitem.TotalHours)
                    End If
                End If
            End If
        Next

        Dim curType As HelperLists.WorkTimeClassInfo

        For i As Integer = 1 To 31

            For Each item As Workers.WorkTimeItem In sheet.GeneralItemList

                curType = Nothing

                Try
                    curType = DirectCast(GetType(Workers.WorkTimeItem).GetProperty( _
                        "DayType" & i.ToString).GetValue(item, Nothing), HelperLists.WorkTimeClassInfo)
                Catch ex As Exception
                End Try

                If Not curType Is Nothing AndAlso curType.ID > 0 AndAlso _
                    curType.ID <> sheet.DefaultPublicHolidayTimeClass.ID AndAlso _
                    curType.ID <> sheet.DefaultRestTimeClass.ID AndAlso _
                    (curType.Type = Workers.WorkTimeType.AnnualHolidays OrElse _
                    curType.Type = Workers.WorkTimeType.DownTime OrElse _
                    curType.Type = Workers.WorkTimeType.OtherExcluded OrElse _
                    curType.Type = Workers.WorkTimeType.OtherHolidays OrElse _
                    curType.Type = Workers.WorkTimeType.SickDays OrElse _
                    curType.Type = Workers.WorkTimeType.Truancy) Then

                    If resultDays.ContainsKey(curType.Code.Trim.ToUpper) Then
                        resultDays(curType.Code.Trim.ToUpper) = _
                            resultDays(curType.Code.Trim.ToUpper) + 1
                    Else
                        resultDays.Add(curType.Code.Trim.ToUpper, 1)
                    End If

                End If

            Next

        Next

        Dim dt As New DataTable
        dt.Rows.Add()
        dt.Rows.Add()
        dt.Rows.Add()

        dt.Columns.Add()
        dt.Rows(0).Item(0) = "Tarnybinės komandiruotės ir neatvykimo į darbą atvejai per mėnesį"
        dt.Rows(1).Item(0) = "Dienos"
        dt.Rows(2).Item(0) = "Valandos"

        For Each k As KeyValuePair(Of String, Double) In resultHours

            dt.Columns.Add()
            dt.Rows(0).Item(dt.Columns.Count - 1) = k.Key
            dt.Rows(2).Item(dt.Columns.Count - 1) = k.Value.ToString

        Next

        For Each k As KeyValuePair(Of String, Integer) In resultDays

            dt.Columns.Add()
            dt.Rows(0).Item(dt.Columns.Count - 1) = k.Key
            dt.Rows(1).Item(dt.Columns.Count - 1) = k.Value.ToString

        Next

        CopyDataTableValues(dt, RD.Table2)

        Return dt.Columns.Count

    End Function

#End Region

#End Region

#Region "Goods"

    ''' <summary>
    ''' Map <see cref="Goods.GoodsComplexOperationDiscard">GoodsComplexOperationDiscard</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Goods.GoodsComplexOperationDiscard, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.Date.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.Content
        RD.TableGeneral.Item(0).Column3 = R_Obj.DocumentNumber
        If Not R_Obj.Warehouse Is Nothing AndAlso R_Obj.Warehouse.ID > 0 Then
            RD.TableGeneral.Item(0).Column4 = R_Obj.Warehouse.Name
            RD.TableGeneral.Item(0).Column5 = R_Obj.Warehouse.WarehouseAccount.ToString
        End If

        Dim totalAmount As Double = 0
        Dim totalSum As Double = 0

        For Each item As Goods.GoodsDiscardItem In R_Obj.GetSortedList

            RD.Table1.Rows.Add()

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.ID.ToString
            If Not item.GoodsInfo Is Nothing AndAlso item.GoodsInfo.ID > 0 Then
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.GoodsInfo.ID.ToString
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.GoodsInfo.Name
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.GoodsInfo.GroupName
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = item.GoodsInfo.MeasureUnit
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = item.GoodsInfo.AccountingMethodHumanReadable
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = item.GoodsInfo.ValuationMethodHumanReadable
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = DblParser(item.GoodsInfo.PricePurchase, ROUNDUNITGOODS)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = DblParser(item.GoodsInfo.PriceSale, ROUNDUNITGOODS)
            End If
            If item.UnitCost > 0 Then
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = DblParser(item.UnitCost, ROUNDUNITGOODS)
            End If
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = DblParser(item.Amount, ROUNDAMOUNTGOODS)
            If item.TotalCost > 0 Then
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(item.TotalCost, 2)
            End If
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = DblParser(0, ROUNDUNITGOODS) ' NormativeUnitValue
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(0, 2) ' NormativeTotalValue
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = item.AccountCost.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = item.Remarks

            totalAmount = CRound(totalAmount + item.Amount, ROUNDAMOUNTGOODS)
            totalSum = CRound(totalSum + item.TotalCost, 2)

        Next

        RD.TableGeneral.Item(0).Column6 = DblParser(totalAmount, ROUNDUNITGOODS)
        If CRound(totalSum, 2) > 0 Then
            RD.TableGeneral.Item(0).Column7 = DblParser(totalSum, 2)
        End If


        ReportFileName = "R_GoodsComplexOperationDiscard.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Goods.GoodsOperationDiscard">GoodsOperationDiscard</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Goods.GoodsOperationDiscard, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.Date.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.Description
        RD.TableGeneral.Item(0).Column3 = R_Obj.DocumentNumber
        If Not R_Obj.Warehouse Is Nothing AndAlso R_Obj.Warehouse.ID > 0 Then
            RD.TableGeneral.Item(0).Column4 = R_Obj.Warehouse.Name
            RD.TableGeneral.Item(0).Column5 = R_Obj.Warehouse.WarehouseAccount.ToString
        End If

        RD.Table1.Rows.Add()

        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = R_Obj.ID.ToString
        If Not R_Obj.GoodsInfo Is Nothing AndAlso R_Obj.GoodsInfo.ID > 0 Then
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = R_Obj.GoodsInfo.ID.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = R_Obj.GoodsInfo.Name
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = R_Obj.GoodsInfo.GroupName
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = R_Obj.GoodsInfo.MeasureUnit
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = R_Obj.GoodsInfo.AccountingMethodHumanReadable
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = R_Obj.GoodsInfo.ValuationMethodHumanReadable
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = DblParser(R_Obj.GoodsInfo.PricePurchase, ROUNDUNITGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = DblParser(R_Obj.GoodsInfo.PriceSale, ROUNDUNITGOODS)
        End If
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = DblParser(R_Obj.UnitCost, ROUNDUNITGOODS)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = DblParser(R_Obj.Ammount, ROUNDAMOUNTGOODS)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(R_Obj.TotalCost, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = DblParser(R_Obj.NormativeUnitValue, ROUNDUNITGOODS)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(R_Obj.NormativeTotalValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = R_Obj.AccountGoodsDiscardCosts.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = R_Obj.Description

        ReportFileName = "R_GoodsOperationDiscard.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Goods.GoodsComplexOperationInternalTransfer">GoodsComplexOperationInternalTransfer</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Goods.GoodsComplexOperationInternalTransfer, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.Date.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.Content
        RD.TableGeneral.Item(0).Column3 = R_Obj.DocumentNumber
        If Not R_Obj.WarehouseFrom Is Nothing AndAlso R_Obj.WarehouseFrom.ID > 0 Then
            RD.TableGeneral.Item(0).Column4 = R_Obj.WarehouseFrom.Name
            RD.TableGeneral.Item(0).Column5 = R_Obj.WarehouseFrom.WarehouseAccount.ToString
        End If
        If Not R_Obj.WarehouseTo Is Nothing AndAlso R_Obj.WarehouseTo.ID > 0 Then
            RD.TableGeneral.Item(0).Column6 = R_Obj.WarehouseTo.Name
            RD.TableGeneral.Item(0).Column7 = R_Obj.WarehouseTo.WarehouseAccount.ToString
        End If

        For Each item As Goods.GoodsInternalTransferItem In R_Obj.GetSortedList

            RD.Table1.Rows.Add()

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.ID.ToString
            If Not item.GoodsInfo Is Nothing AndAlso item.GoodsInfo.ID > 0 Then
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.GoodsInfo.ID.ToString
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.GoodsInfo.Name
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.GoodsInfo.GroupName
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = item.GoodsInfo.MeasureUnit
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = item.GoodsInfo.AccountingMethodHumanReadable
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = item.GoodsInfo.ValuationMethodHumanReadable
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = DblParser(item.GoodsInfo.PricePurchase, ROUNDUNITGOODS)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = DblParser(item.GoodsInfo.PriceSale, ROUNDUNITGOODS)
            End If
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = DblParser(item.UnitCost, ROUNDUNITGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = DblParser(item.Amount, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(item.TotalCost, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = item.Remarks

        Next

        ReportFileName = "R_GoodsOperationInternalTransfer.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Goods.GoodsOperationTransfer">GoodsOperationTransfer</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Goods.GoodsOperationTransfer, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.Date.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.Description
        RD.TableGeneral.Item(0).Column3 = R_Obj.DocumentNumber
        If Not R_Obj.Warehouse Is Nothing AndAlso R_Obj.Warehouse.ID > 0 Then
            RD.TableGeneral.Item(0).Column4 = R_Obj.Warehouse.Name
            RD.TableGeneral.Item(0).Column5 = R_Obj.Warehouse.WarehouseAccount.ToString
        End If
        RD.TableGeneral.Item(0).Column6 = R_Obj.JournalEntryDate.ToShortDateString
        RD.TableGeneral.Item(0).Column7 = R_Obj.JournalEntryTypeHumanReadable
        RD.TableGeneral.Item(0).Column8 = R_Obj.JournalEntryRelatedPerson
        RD.TableGeneral.Item(0).Column9 = R_Obj.JournalEntryContent
        RD.TableGeneral.Item(0).Column10 = R_Obj.JournalEntryCorrespondence

        RD.Table1.Rows.Add()

        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = R_Obj.ID.ToString
        If Not R_Obj.GoodsInfo Is Nothing AndAlso R_Obj.GoodsInfo.ID > 0 Then
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = R_Obj.GoodsInfo.ID.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = R_Obj.GoodsInfo.Name
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = R_Obj.GoodsInfo.GroupName
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = R_Obj.GoodsInfo.MeasureUnit
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = R_Obj.GoodsInfo.AccountingMethodHumanReadable
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = R_Obj.GoodsInfo.ValuationMethodHumanReadable
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = DblParser(R_Obj.GoodsInfo.PricePurchase, ROUNDUNITGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = DblParser(R_Obj.GoodsInfo.PriceSale, ROUNDUNITGOODS)
        End If
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = DblParser(R_Obj.UnitCost, ROUNDUNITGOODS)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = DblParser(R_Obj.Amount, ROUNDAMOUNTGOODS)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(R_Obj.TotalCost, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = R_Obj.AccountGoodsCost.ToString

        ReportFileName = "R_GoodsOperationTransfer.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Goods.GoodsComplexOperationInventorization">GoodsComplexOperationInventorization</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Goods.GoodsComplexOperationInventorization, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.Date.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.Content
        RD.TableGeneral.Item(0).Column3 = R_Obj.DocumentNumber
        RD.TableGeneral.Item(0).Column4 = R_Obj.WarehouseName
        RD.TableGeneral.Item(0).Column5 = R_Obj.WarehouseAccount.ToString

        For Each item As Goods.GoodsInventorizationItem In R_Obj.GetSortedList

            RD.Table1.Rows.Add()

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.ID.ToString
            If Not item.GoodsInfo Is Nothing AndAlso item.GoodsInfo.ID > 0 Then
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.GoodsInfo.ID.ToString
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.GoodsInfo.Name
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.GoodsInfo.GroupName
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = item.GoodsInfo.MeasureUnit
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = item.GoodsInfo.AccountingMethodHumanReadable
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = item.GoodsInfo.ValuationMethodHumanReadable
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = DblParser(item.GoodsInfo.PricePurchase, ROUNDUNITGOODS)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = DblParser(item.GoodsInfo.PriceSale, ROUNDUNITGOODS)
            End If

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = DblParser(item.UnitValueLastInventorization, ROUNDUNITGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = DblParser(item.AmountLastInventorization, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(item.TotalValueLastInventorization, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = DblParser(item.AmountAcquisitions, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(item.TotalValueAcquisitions, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = DblParser(item.AmountTransfered, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = DblParser(item.TotalValueTransfered, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = DblParser(item.AmountDisposed, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = DblParser(item.TotalValueDisposed, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = DblParser(item.TotalValueDiscount, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column20 = DblParser(item.TotalValueAdditionalCosts, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column21 = DblParser(item.UnitValueCalculated, ROUNDUNITGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column22 = DblParser(item.AmountCalculated, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column23 = DblParser(item.TotalValueCalculated, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column24 = DblParser(item.AmountChange, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column25 = item.AccountCorresponding.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column26 = DblParser(item.AmountAfterInventorization, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column27 = DblParser(item.TotalValueAfterInventorization, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column28 = item.Remarks

        Next

        ReportFileName = "R_GoodsOperationInventorization.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Goods.GoodsComplexOperationPriceCut">GoodsComplexOperationPriceCut</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Goods.GoodsComplexOperationPriceCut, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.Date.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.Description
        RD.TableGeneral.Item(0).Column3 = R_Obj.DocumentNumber

        For Each item As Goods.GoodsPriceCutItem In R_Obj.GetSortedList

            RD.Table1.Rows.Add()

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.ID.ToString
            If Not item.GoodsInfo Is Nothing AndAlso item.GoodsInfo.ID > 0 Then
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.GoodsInfo.ID.ToString
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.GoodsInfo.Name
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.GoodsInfo.GroupName
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = item.GoodsInfo.MeasureUnit
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = item.GoodsInfo.AccountingMethodHumanReadable
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = item.GoodsInfo.ValuationMethodHumanReadable
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = DblParser(item.GoodsInfo.PricePurchase, ROUNDUNITGOODS)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = DblParser(item.GoodsInfo.PriceSale, ROUNDUNITGOODS)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = item.GoodsInfo.AccountValueReduction.ToString
            End If

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = DblParser(item.AmountInWarehouseAccounts, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(item.UnitValueInWarehouseAccounts, ROUNDUNITGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = DblParser(item.TotalValueInWarehouseAccounts, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(item.TotalValueCurrentPriceCut, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = DblParser(item.UnitValuePriceCut, ROUNDUNITGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = DblParser(item.TotalValuePriceCut, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = item.AccountPriceCutCosts.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = DblParser(item.UnitValueAfterPriceCut, ROUNDUNITGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = DblParser(item.TotalValueAfterPriceCut, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column20 = item.Remarks

        Next

        ReportFileName = "R_GoodsOperationPriceCut.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Goods.GoodsOperationPriceCut">GoodsOperationPriceCut</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Goods.GoodsOperationPriceCut, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.Date.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.Description
        RD.TableGeneral.Item(0).Column3 = R_Obj.DocumentNumber

        RD.Table1.Rows.Add()

        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = R_Obj.ID.ToString
        If Not R_Obj.GoodsInfo Is Nothing AndAlso R_Obj.GoodsInfo.ID > 0 Then
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = R_Obj.GoodsInfo.ID.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = R_Obj.GoodsInfo.Name
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = R_Obj.GoodsInfo.GroupName
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = R_Obj.GoodsInfo.MeasureUnit
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = R_Obj.GoodsInfo.AccountingMethodHumanReadable
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = R_Obj.GoodsInfo.ValuationMethodHumanReadable
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = DblParser(R_Obj.GoodsInfo.PricePurchase, ROUNDUNITGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = DblParser(R_Obj.GoodsInfo.PriceSale, ROUNDUNITGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = R_Obj.GoodsInfo.AccountValueReduction.ToString
        End If

        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = DblParser(R_Obj.AmountInWarehouseAccounts, ROUNDAMOUNTGOODS)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(R_Obj.UnitValueInWarehouseAccounts, ROUNDUNITGOODS)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = DblParser(R_Obj.TotalValueInWarehouseAccounts, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(R_Obj.TotalValueCurrentPriceCut, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = DblParser(R_Obj.UnitValuePriceCut, ROUNDUNITGOODS)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = DblParser(R_Obj.TotalValuePriceCut, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = R_Obj.AccountPriceCutCosts.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = DblParser(R_Obj.UnitValueAfterPriceCut, ROUNDUNITGOODS)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = DblParser(R_Obj.TotalValueAfterPriceCut, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column20 = R_Obj.Description

        ReportFileName = "R_GoodsOperationPriceCut.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Goods.GoodsComplexOperationProduction">GoodsComplexOperationProduction</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Goods.GoodsComplexOperationProduction, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.Date.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.Content
        RD.TableGeneral.Item(0).Column3 = R_Obj.DocumentNumber

        If Not R_Obj.GoodsInfo Is Nothing AndAlso R_Obj.GoodsInfo.ID > 0 Then
            RD.TableGeneral.Item(0).Column4 = R_Obj.GoodsInfo.ID.ToString
            RD.TableGeneral.Item(0).Column5 = R_Obj.GoodsInfo.Name
            RD.TableGeneral.Item(0).Column6 = R_Obj.GoodsInfo.GroupName
            RD.TableGeneral.Item(0).Column7 = R_Obj.GoodsInfo.MeasureUnit
            RD.TableGeneral.Item(0).Column8 = R_Obj.GoodsInfo.AccountingMethodHumanReadable
            RD.TableGeneral.Item(0).Column9 = R_Obj.GoodsInfo.ValuationMethodHumanReadable
            RD.TableGeneral.Item(0).Column10 = DblParser(R_Obj.GoodsInfo.PricePurchase, ROUNDUNITGOODS)
            RD.TableGeneral.Item(0).Column11 = DblParser(R_Obj.GoodsInfo.PriceSale, ROUNDUNITGOODS)
        End If

        RD.TableGeneral.Item(0).Column12 = DblParser(R_Obj.UnitValue, ROUNDUNITGOODS)
        RD.TableGeneral.Item(0).Column13 = DblParser(R_Obj.Amount, ROUNDAMOUNTGOODS)
        RD.TableGeneral.Item(0).Column14 = DblParser(R_Obj.TotalValue, 2)

        If Not R_Obj.WarehouseForProduction Is Nothing AndAlso R_Obj.WarehouseForProduction.ID > 0 Then
            RD.TableGeneral.Item(0).Column15 = R_Obj.WarehouseForProduction.Name
            RD.TableGeneral.Item(0).Column16 = R_Obj.WarehouseForProduction.WarehouseAccount.ToString
        End If
        If Not R_Obj.WarehouseForComponents Is Nothing AndAlso R_Obj.WarehouseForComponents.ID > 0 Then
            RD.TableGeneral.Item(0).Column17 = R_Obj.WarehouseForComponents.Name
            RD.TableGeneral.Item(0).Column18 = R_Obj.WarehouseForComponents.WarehouseAccount.ToString
        End If

        RD.TableGeneral.Item(0).Column19 = DblParser(R_Obj.CalculationIsPerUnit, ROUNDUNITGOODS)

        For Each item As Goods.GoodsComponentItem In R_Obj.GetComponentSortedList

            RD.Table1.Rows.Add()

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.ID.ToString
            If Not item.GoodsInfo Is Nothing AndAlso item.GoodsInfo.ID > 0 Then
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.GoodsInfo.ID.ToString
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.GoodsInfo.Name
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.GoodsInfo.GroupName
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = item.GoodsInfo.MeasureUnit
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = item.GoodsInfo.AccountingMethodHumanReadable
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = item.GoodsInfo.ValuationMethodHumanReadable
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = DblParser(item.GoodsInfo.PricePurchase, ROUNDUNITGOODS)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = DblParser(item.GoodsInfo.PriceSale, ROUNDUNITGOODS)
            End If

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = DblParser(item.AmountPerProductionUnit, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = DblParser(item.Amount, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(item.NormativeUnitCost, ROUNDUNITGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = item.AccountContrary.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(item.UnitCost, ROUNDUNITGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = DblParser(item.TotalCost, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = item.Remarks

        Next

        For Each item As Goods.GoodsProductionCostItem In R_Obj.GetCostsSortedList

            RD.Table2.Rows.Add()

            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column1 = item.Account.ToString
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column2 = DblParser(item.CostPerProductionUnit, ROUNDUNITGOODS)
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column3 = DblParser(item.TotalCost, 2)

        Next

        ReportFileName = "R_GoodsOperationProduction.rdlc"
        NumberOfTablesInUse = 2

    End Sub

    ''' <summary>
    ''' Map <see cref="Goods.GoodsOperationAdditionalCosts">GoodsOperationAdditionalCosts</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Goods.GoodsOperationAdditionalCosts, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)


        RD.TableGeneral.Item(0).Column1 = R_Obj.Date.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.Description
        RD.TableGeneral.Item(0).Column3 = R_Obj.DocumentNumber

        If Not R_Obj.GoodsInfo Is Nothing AndAlso R_Obj.GoodsInfo.ID > 0 Then
            RD.TableGeneral.Item(0).Column4 = R_Obj.GoodsInfo.ID.ToString
            RD.TableGeneral.Item(0).Column5 = R_Obj.GoodsInfo.Name
            RD.TableGeneral.Item(0).Column6 = R_Obj.GoodsInfo.GroupName
            RD.TableGeneral.Item(0).Column7 = R_Obj.GoodsInfo.MeasureUnit
            RD.TableGeneral.Item(0).Column8 = R_Obj.GoodsInfo.AccountingMethodHumanReadable
            RD.TableGeneral.Item(0).Column9 = R_Obj.GoodsInfo.ValuationMethodHumanReadable
            RD.TableGeneral.Item(0).Column10 = DblParser(R_Obj.GoodsInfo.PricePurchase, ROUNDUNITGOODS)
            RD.TableGeneral.Item(0).Column11 = DblParser(R_Obj.GoodsInfo.PriceSale, ROUNDUNITGOODS)
        End If

        If Not R_Obj.Warehouse Is Nothing AndAlso R_Obj.Warehouse.ID > 0 Then
            RD.TableGeneral.Item(0).Column12 = R_Obj.Warehouse.Name
            RD.TableGeneral.Item(0).Column13 = R_Obj.Warehouse.WarehouseAccount.ToString
        End If

        RD.TableGeneral.Item(0).Column14 = DblParser(R_Obj.TotalGoodsValueChange, 2)
        RD.TableGeneral.Item(0).Column15 = DblParser(R_Obj.TotalNetValueChange, 2)
        RD.TableGeneral.Item(0).Column16 = DblParser(R_Obj.TotalValueChange, 2)
        RD.TableGeneral.Item(0).Column17 = BooleanToCheckMark(R_Obj.AccountPurchasesIsClosed)
        RD.TableGeneral.Item(0).Column18 = R_Obj.AccountGoodsGeneral.ToString
        RD.TableGeneral.Item(0).Column19 = R_Obj.AccountGoodsNetCosts.ToString
        RD.TableGeneral.Item(0).Column20 = R_Obj.JournalEntryTypeHumanReadable.ToString
        RD.TableGeneral.Item(0).Column21 = R_Obj.JournalEntryContent.ToString
        RD.TableGeneral.Item(0).Column22 = R_Obj.JournalEntryCorrespondence.ToString
        RD.TableGeneral.Item(0).Column23 = R_Obj.JournalEntryRelatedPerson.ToString

        For Each item As Goods.ConsignmentItem In R_Obj.GetSortedList

            RD.Table1.Rows.Add()

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.ID.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.AcquisitionID.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.AcquisitionDate.ToShortDateString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.AcquisitionDocTypeHumanReadable
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = item.AcquisitionDocNo
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = DblParser(item.Amount, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = DblParser(item.UnitValue, ROUNDUNITGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = DblParser(item.TotalValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = DblParser(item.AmountWithdrawn, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = DblParser(item.TotalValueWithdrawn, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = DblParser(item.AmountLeft, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(item.TotalValueLeft, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = DblParser(item.UnitValueChange, ROUNDUNITGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(item.TotalValueChange, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = item.WarehouseName

        Next

        ReportFileName = "R_GoodsOperationAdditionalCosts.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Goods.GoodsOperationDiscount">GoodsOperationDiscount</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Goods.GoodsOperationDiscount, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.Date.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.Description
        RD.TableGeneral.Item(0).Column3 = R_Obj.DocumentNumber

        If Not R_Obj.GoodsInfo Is Nothing AndAlso R_Obj.GoodsInfo.ID > 0 Then
            RD.TableGeneral.Item(0).Column4 = R_Obj.GoodsInfo.ID.ToString
            RD.TableGeneral.Item(0).Column5 = R_Obj.GoodsInfo.Name
            RD.TableGeneral.Item(0).Column6 = R_Obj.GoodsInfo.GroupName
            RD.TableGeneral.Item(0).Column7 = R_Obj.GoodsInfo.MeasureUnit
            RD.TableGeneral.Item(0).Column8 = R_Obj.GoodsInfo.AccountingMethodHumanReadable
            RD.TableGeneral.Item(0).Column9 = R_Obj.GoodsInfo.ValuationMethodHumanReadable
            RD.TableGeneral.Item(0).Column10 = DblParser(R_Obj.GoodsInfo.PricePurchase, ROUNDUNITGOODS)
            RD.TableGeneral.Item(0).Column11 = DblParser(R_Obj.GoodsInfo.PriceSale, ROUNDUNITGOODS)
        End If

        If Not R_Obj.Warehouse Is Nothing AndAlso R_Obj.Warehouse.ID > 0 Then
            RD.TableGeneral.Item(0).Column12 = R_Obj.Warehouse.Name
            RD.TableGeneral.Item(0).Column13 = R_Obj.Warehouse.WarehouseAccount.ToString
        End If

        RD.TableGeneral.Item(0).Column14 = DblParser(R_Obj.TotalGoodsValueChange, 2)
        RD.TableGeneral.Item(0).Column15 = DblParser(R_Obj.TotalNetValueChange, 2)
        RD.TableGeneral.Item(0).Column16 = DblParser(R_Obj.TotalValueChange, 2)
        RD.TableGeneral.Item(0).Column17 = BooleanToCheckMark(R_Obj.AccountPurchasesIsClosed)
        RD.TableGeneral.Item(0).Column18 = R_Obj.AccountGoodsGeneral.ToString
        RD.TableGeneral.Item(0).Column19 = R_Obj.AccountGoodsNetCosts.ToString
        RD.TableGeneral.Item(0).Column20 = R_Obj.JournalEntryTypeHumanReadable.ToString
        RD.TableGeneral.Item(0).Column21 = R_Obj.JournalEntryContent.ToString
        RD.TableGeneral.Item(0).Column22 = R_Obj.JournalEntryCorrespondence.ToString
        RD.TableGeneral.Item(0).Column23 = R_Obj.JournalEntryRelatedPerson.ToString

        For Each item As Goods.ConsignmentItem In R_Obj.GetSortedList

            RD.Table1.Rows.Add()

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.ID.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.AcquisitionID.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.AcquisitionDate.ToShortDateString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.AcquisitionDocTypeHumanReadable
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = item.AcquisitionDocNo
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = DblParser(item.Amount, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = DblParser(item.UnitValue, ROUNDUNITGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = DblParser(item.TotalValue, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = DblParser(item.AmountWithdrawn, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = DblParser(item.TotalValueWithdrawn, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = DblParser(item.AmountLeft, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(item.TotalValueLeft, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = DblParser(item.UnitValueChange, ROUNDUNITGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(item.TotalValueChange, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = item.WarehouseName

        Next

        ReportFileName = "R_GoodsOperationDiscount.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Goods.GoodsOperationAccountChange">GoodsOperationAccountChange</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Goods.GoodsOperationAccountChange, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.Date.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.Description
        RD.TableGeneral.Item(0).Column3 = R_Obj.DocumentNumber

        RD.Table1.Rows.Add()

        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = R_Obj.ID.ToString
        If Not R_Obj.GoodsInfo Is Nothing AndAlso R_Obj.GoodsInfo.ID > 0 Then
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = R_Obj.GoodsInfo.ID.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = R_Obj.GoodsInfo.Name
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = R_Obj.GoodsInfo.GroupName
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = R_Obj.GoodsInfo.MeasureUnit
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = R_Obj.GoodsInfo.AccountingMethodHumanReadable
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = R_Obj.GoodsInfo.ValuationMethodHumanReadable
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = DblParser(R_Obj.GoodsInfo.PricePurchase, ROUNDUNITGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = DblParser(R_Obj.GoodsInfo.PriceSale, ROUNDUNITGOODS)
        End If
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = R_Obj.TypeHumanReadable
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = R_Obj.PreviousAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(R_Obj.CorrespondationValue, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = R_Obj.NewAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = R_Obj.Description

        ReportFileName = "R_GoodsOperationAccountChange.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Goods.GoodsOperationValuationMethod">GoodsOperationValuationMethod</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Goods.GoodsOperationValuationMethod, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.Date.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.Description

        RD.Table1.Rows.Add()

        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = R_Obj.ID.ToString
        If Not R_Obj.GoodsInfo Is Nothing AndAlso R_Obj.GoodsInfo.ID > 0 Then
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = R_Obj.GoodsInfo.ID.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = R_Obj.GoodsInfo.Name
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = R_Obj.GoodsInfo.GroupName
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = R_Obj.GoodsInfo.MeasureUnit
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = R_Obj.GoodsInfo.AccountingMethodHumanReadable
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = DblParser(R_Obj.GoodsInfo.PricePurchase, ROUNDUNITGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = DblParser(R_Obj.GoodsInfo.PriceSale, ROUNDUNITGOODS)
        End If
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = R_Obj.PreviousMethodHumanReadable
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = R_Obj.NewMethodHumanReadable
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = R_Obj.Description

        ReportFileName = "R_GoodsOperationAccountChange.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Goods.GoodsOperationAcquisition">GoodsOperationAcquisition</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Goods.GoodsOperationAcquisition, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.Date.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.Description
        RD.TableGeneral.Item(0).Column3 = R_Obj.DocumentNumber

        If Not R_Obj.Warehouse Is Nothing AndAlso R_Obj.Warehouse.ID > 0 Then
            RD.TableGeneral.Item(0).Column4 = R_Obj.Warehouse.Name
            RD.TableGeneral.Item(0).Column5 = R_Obj.Warehouse.WarehouseAccount.ToString
        End If

        RD.Table1.Rows.Add()

        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = R_Obj.ID.ToString
        If Not R_Obj.GoodsInfo Is Nothing AndAlso R_Obj.GoodsInfo.ID > 0 Then
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = R_Obj.GoodsInfo.ID.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = R_Obj.GoodsInfo.Name
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = R_Obj.GoodsInfo.GroupName
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = R_Obj.GoodsInfo.MeasureUnit
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = R_Obj.GoodsInfo.AccountingMethodHumanReadable
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = R_Obj.GoodsInfo.ValuationMethodHumanReadable
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = DblParser(R_Obj.GoodsInfo.PricePurchase, ROUNDUNITGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = DblParser(R_Obj.GoodsInfo.PriceSale, ROUNDUNITGOODS)
        End If
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = R_Obj.AcquisitionAccount.ToString
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = DblParser(R_Obj.UnitCost, ROUNDUNITGOODS)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(R_Obj.Ammount, ROUNDAMOUNTGOODS)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = DblParser(R_Obj.TotalCost, 2)
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = R_Obj.JournalEntryTypeHumanReadable
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = R_Obj.JournalEntryContent
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = R_Obj.JournalEntryRelatedPerson
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = R_Obj.JournalEntryCorrespondence
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = R_Obj.JournalEntryCorrespondence
        RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = R_Obj.Description

        ReportFileName = "R_GoodsOperationAcquisition.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="Goods.ProductionCalculation">ProductionCalculation</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As Goods.ProductionCalculation, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.Date.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.Description
        RD.TableGeneral.Item(0).Column3 = DblParser(R_Obj.Amount, ROUNDAMOUNTGOODS)

        If Not R_Obj.Goods Is Nothing AndAlso R_Obj.Goods.ID > 0 Then
            RD.TableGeneral.Item(0).Column4 = R_Obj.Goods.ID.ToString
            RD.TableGeneral.Item(0).Column5 = R_Obj.Goods.Name
            RD.TableGeneral.Item(0).Column6 = R_Obj.Goods.MeasureUnit
            RD.TableGeneral.Item(0).Column7 = R_Obj.Goods.GoodsCode
            RD.TableGeneral.Item(0).Column8 = R_Obj.Goods.GoodsBarcode
        End If

        RD.TableGeneral.Item(0).Column9 = BooleanToCheckMark(R_Obj.IsObsolete)

        For Each item As Goods.ProductionComponentItem In R_Obj.ComponentListSorted

            RD.Table1.Rows.Add()

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.ID.ToString
            If Not item.Goods Is Nothing AndAlso item.Goods.ID > 0 Then
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.Goods.ID.ToString
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.Goods.Name
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.Goods.MeasureUnit
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = item.Goods.GoodsCode
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = item.Goods.GoodsBarcode
            End If

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = DblParser(item.Amount, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = DblParser(item.NormativeUnitCost, ROUNDUNITGOODS)

        Next

        For Each item As Goods.ProductionCostItem In R_Obj.CostListSorted

            RD.Table2.Rows.Add()

            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column1 = item.ID.ToString
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column2 = item.Account.ToString
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column3 = DblParser(item.Amount, ROUNDUNITGOODS)

        Next

        ReportFileName = "R_GoodsProductionCalculation.rdlc"
        NumberOfTablesInUse = 2

    End Sub

    ''' <summary>
    ''' Map <see cref="ActiveReports.GoodsOperationInfoListParent">GoodsOperationInfoListParent</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As ActiveReports.GoodsOperationInfoListParent, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.DateFrom.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.DateTo.ToShortDateString

        If Not R_Obj.Warehouse Is Nothing AndAlso R_Obj.Warehouse.ID > 0 Then
            RD.TableGeneral.Item(0).Column3 = R_Obj.Warehouse.Name
            RD.TableGeneral.Item(0).Column4 = R_Obj.Warehouse.WarehouseAccount.ToString
        End If

        RD.Table1.Rows.Add()

        If Not R_Obj.GoodsTurnoverInfo Is Nothing AndAlso R_Obj.GoodsTurnoverInfo.ID > 0 Then

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = R_Obj.GoodsTurnoverInfo.ID.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = R_Obj.GoodsTurnoverInfo.Name
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = R_Obj.GoodsTurnoverInfo.GroupName
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = R_Obj.GoodsTurnoverInfo.MeasureUnit
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = R_Obj.GoodsTurnoverInfo.BarCode
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = R_Obj.GoodsTurnoverInfo.Code
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = BooleanToCheckMark(R_Obj.GoodsTurnoverInfo.IsObsolete)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = R_Obj.GoodsTurnoverInfo.TradeType
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = DblParser(R_Obj.GoodsTurnoverInfo.DefaultVatRatePurchase, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = DblParser(R_Obj.GoodsTurnoverInfo.DefaultVatRateSales, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = DblParser(R_Obj.GoodsTurnoverInfo.PricePurchase, ROUNDAMOUNTINVOICERECEIVED)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(R_Obj.GoodsTurnoverInfo.PriceSale, ROUNDAMOUNTINVOICEMADE)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = R_Obj.GoodsTurnoverInfo.AccountDiscounts.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = R_Obj.GoodsTurnoverInfo.AccountPurchases.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = R_Obj.GoodsTurnoverInfo.AccountSalesNetCosts.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = R_Obj.GoodsTurnoverInfo.AccountValueReduction.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = R_Obj.GoodsTurnoverInfo.AccountingMethod
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = R_Obj.GoodsTurnoverInfo.ValuationMethod
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = DblParser(R_Obj.GoodsTurnoverInfo.AmountPeriodStart, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column20 = DblParser(R_Obj.GoodsTurnoverInfo.UnitValuePeriodStart, ROUNDUNITGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column21 = DblParser(R_Obj.GoodsTurnoverInfo.TotalValuePeriodStart, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column22 = DblParser(R_Obj.GoodsTurnoverInfo.AmountInWarehousePeriodStart, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column23 = DblParser(R_Obj.GoodsTurnoverInfo.UnitValueInWarehousePeriodStart, ROUNDUNITGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column24 = DblParser(R_Obj.GoodsTurnoverInfo.TotalValueInWarehousePeriodStart, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column25 = DblParser(R_Obj.GoodsTurnoverInfo.AmountPurchasesPeriodStart, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column26 = DblParser(R_Obj.GoodsTurnoverInfo.AmountPendingPeriodStart, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column27 = DblParser(R_Obj.GoodsTurnoverInfo.AmountAcquisitions, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column28 = DblParser(R_Obj.GoodsTurnoverInfo.AmountAcquisitionsInWarehouse, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column29 = DblParser(R_Obj.GoodsTurnoverInfo.AmountDiscarded, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column30 = DblParser(R_Obj.GoodsTurnoverInfo.AmountDiscardedInWarehouse, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column31 = DblParser(R_Obj.GoodsTurnoverInfo.AmountTransfered, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column32 = DblParser(R_Obj.GoodsTurnoverInfo.AmountTransferedInWarehouse, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column33 = DblParser(R_Obj.GoodsTurnoverInfo.AmountChangeInventorization, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column34 = DblParser(R_Obj.GoodsTurnoverInfo.TotalAdditionalCosts, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column35 = DblParser(R_Obj.GoodsTurnoverInfo.TotalAdditionalCostsForDiscardedGoods, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column36 = DblParser(R_Obj.GoodsTurnoverInfo.TotalDiscounts, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column37 = DblParser(R_Obj.GoodsTurnoverInfo.TotalDiscountsForDiscardedGoods, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column38 = DblParser(R_Obj.GoodsTurnoverInfo.AmountPeriodEnd, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column39 = DblParser(R_Obj.GoodsTurnoverInfo.UnitValuePeriodEnd, ROUNDUNITGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column40 = DblParser(R_Obj.GoodsTurnoverInfo.TotalValuePeriodEnd, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column41 = DblParser(R_Obj.GoodsTurnoverInfo.AmountInWarehousePeriodEnd, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column42 = DblParser(R_Obj.GoodsTurnoverInfo.UnitValueInWarehousePeriodEnd, ROUNDUNITGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column43 = DblParser(R_Obj.GoodsTurnoverInfo.TotalValueInWarehousePeriodEnd, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column44 = DblParser(R_Obj.GoodsTurnoverInfo.AmountPurchasesPeriodEnd, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column45 = DblParser(R_Obj.GoodsTurnoverInfo.AmountPendingPeriodEnd, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column46 = DblParser(R_Obj.GoodsTurnoverInfo.AccountWarehousePeriodStart, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column47 = DblParser(R_Obj.GoodsTurnoverInfo.AccountWarehouseDebit, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column48 = DblParser(R_Obj.GoodsTurnoverInfo.AccountWarehouseCredit, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column49 = DblParser(R_Obj.GoodsTurnoverInfo.AccountWarehousePeriodEnd, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column50 = DblParser(R_Obj.GoodsTurnoverInfo.AccountPurchasesPeriodStart, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column51 = DblParser(R_Obj.GoodsTurnoverInfo.AccountPurchasesDebit, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column52 = DblParser(R_Obj.GoodsTurnoverInfo.AccountPurchasesCredit, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column53 = DblParser(R_Obj.GoodsTurnoverInfo.AccountPurchasesPeriodEnd, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column54 = DblParser(R_Obj.GoodsTurnoverInfo.AccountSalesNetCostsPeriodStart, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column55 = DblParser(R_Obj.GoodsTurnoverInfo.AccountSalesNetCostsDebit, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column56 = DblParser(R_Obj.GoodsTurnoverInfo.AccountSalesNetCostsCredit, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column57 = DblParser(R_Obj.GoodsTurnoverInfo.AccountSalesNetCostsPeriodEnd, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column58 = DblParser(R_Obj.GoodsTurnoverInfo.AccountDiscountsPeriodStart, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column59 = DblParser(R_Obj.GoodsTurnoverInfo.AccountDiscountsDebit, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column60 = DblParser(R_Obj.GoodsTurnoverInfo.AccountDiscountsCredit, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column61 = DblParser(R_Obj.GoodsTurnoverInfo.AccountDiscountsPeriodEnd, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column62 = DblParser(R_Obj.GoodsTurnoverInfo.AccountValueReductionPeriodStart, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column63 = DblParser(R_Obj.GoodsTurnoverInfo.AccountValueReductionDebit, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column64 = DblParser(R_Obj.GoodsTurnoverInfo.AccountValueReductionCredit, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column65 = DblParser(R_Obj.GoodsTurnoverInfo.AccountValueReductionPeriodEnd, 2)

        End If

        For Each item As ActiveReports.GoodsOperationInfo In R_Obj.GetSortedList

            RD.Table2.Rows.Add()

            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column1 = item.ID.ToString
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column2 = item.Type
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column3 = item.ComplexOperationID.ToString
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column4 = item.ComplexType
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column5 = item.Date.ToShortDateString
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column6 = item.DocNo
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column7 = item.Content
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column8 = item.JournalEntryID.ToString
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column9 = item.JournalEntryType
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column10 = item.JournalEntryDate.ToShortDateString
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column11 = item.JournalEntryDocNo
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column12 = item.JournalEntryContent
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column13 = item.JournalEntryCorrespondentions
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column14 = item.WarehouseName
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column15 = item.WarehouseAccount.ToString
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column16 = DblParser(item.Amount, ROUNDAMOUNTGOODS)
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column17 = DblParser(item.AmountInWarehouse, ROUNDAMOUNTGOODS)
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column18 = DblParser(item.UnitValue, ROUNDUNITGOODS)
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column19 = DblParser(item.TotalValue, 2)
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column20 = DblParser(item.AccountGeneral, 2)
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column21 = DblParser(item.AccountPurchases, 2)
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column22 = DblParser(item.AccountSalesNetCosts, 2)
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column23 = DblParser(item.AccountDiscounts, 2)
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column24 = DblParser(item.AccountPriceCut, 2)
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column25 = item.AccountOperation.ToString
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column26 = DblParser(item.AccountOperationValue, 2)
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column27 = item.InsertDate.ToShortDateString
            RD.Table2.Item(RD.Table2.Rows.Count - 1).Column28 = item.UpdateDate.ToShortDateString

        Next

        ReportFileName = "R_GoodsOperationInfoListParent.rdlc"
        NumberOfTablesInUse = 2

    End Sub

    ''' <summary>
    ''' Map <see cref="ActiveReports.GoodsTurnoverInfoList">GoodsTurnoverInfoList</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As ActiveReports.GoodsTurnoverInfoList, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.DateFrom.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.DateTo.ToShortDateString

        If Not R_Obj.Warehouse Is Nothing AndAlso R_Obj.Warehouse.ID > 0 Then
            RD.TableGeneral.Item(0).Column3 = R_Obj.Warehouse.Name
            RD.TableGeneral.Item(0).Column4 = R_Obj.Warehouse.WarehouseAccount.ToString
        Else
            RD.TableGeneral.Item(0).Column3 = "Visi sandėliai"
            RD.TableGeneral.Item(0).Column4 = ""
        End If

        If Not R_Obj.Group Is Nothing AndAlso R_Obj.Group.ID > 0 Then
            RD.TableGeneral.Item(0).Column5 = R_Obj.Group.Name
        Else
            RD.TableGeneral.Item(0).Column5 = "Visos grupės"
        End If

        For Each item As ActiveReports.GoodsTurnoverInfo In R_Obj.GetSortedList

            RD.Table1.Rows.Add()

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.ID.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.Name
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.GroupName
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.MeasureUnit
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = item.BarCode
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = item.Code
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = BooleanToCheckMark(item.IsObsolete)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = item.TradeType
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = DblParser(item.DefaultVatRatePurchase, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = DblParser(item.DefaultVatRateSales, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = DblParser(item.PricePurchase, ROUNDAMOUNTINVOICERECEIVED)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(item.PriceSale, ROUNDAMOUNTINVOICEMADE)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = item.AccountDiscounts.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = item.AccountPurchases.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = item.AccountSalesNetCosts.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = item.AccountValueReduction.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = item.AccountingMethod
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = item.ValuationMethod
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = DblParser(item.AmountPeriodStart, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column20 = DblParser(item.UnitValuePeriodStart, ROUNDUNITGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column21 = DblParser(item.TotalValuePeriodStart, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column22 = DblParser(item.AmountInWarehousePeriodStart, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column23 = DblParser(item.UnitValueInWarehousePeriodStart, ROUNDUNITGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column24 = DblParser(item.TotalValueInWarehousePeriodStart, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column25 = DblParser(item.AmountPurchasesPeriodStart, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column26 = DblParser(item.AmountPendingPeriodStart, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column27 = DblParser(item.AmountAcquisitions, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column28 = DblParser(item.AmountAcquisitionsInWarehouse, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column29 = DblParser(item.AmountDiscarded, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column30 = DblParser(item.AmountDiscardedInWarehouse, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column31 = DblParser(item.AmountTransfered, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column32 = DblParser(item.AmountTransferedInWarehouse, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column33 = DblParser(item.AmountChangeInventorization, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column34 = DblParser(item.TotalAdditionalCosts, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column35 = DblParser(item.TotalAdditionalCostsForDiscardedGoods, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column36 = DblParser(item.TotalDiscounts, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column37 = DblParser(item.TotalDiscountsForDiscardedGoods, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column38 = DblParser(item.AmountPeriodEnd, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column39 = DblParser(item.UnitValuePeriodEnd, ROUNDUNITGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column40 = DblParser(item.TotalValuePeriodEnd, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column41 = DblParser(item.AmountInWarehousePeriodEnd, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column42 = DblParser(item.UnitValueInWarehousePeriodEnd, ROUNDUNITGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column43 = DblParser(item.TotalValueInWarehousePeriodEnd, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column44 = DblParser(item.AmountPurchasesPeriodEnd, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column45 = DblParser(item.AmountPendingPeriodEnd, ROUNDAMOUNTGOODS)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column46 = DblParser(item.AccountWarehousePeriodStart, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column47 = DblParser(item.AccountWarehouseDebit, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column48 = DblParser(item.AccountWarehouseCredit, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column49 = DblParser(item.AccountWarehousePeriodEnd, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column50 = DblParser(item.AccountPurchasesPeriodStart, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column51 = DblParser(item.AccountPurchasesDebit, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column52 = DblParser(item.AccountPurchasesCredit, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column53 = DblParser(item.AccountPurchasesPeriodEnd, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column54 = DblParser(item.AccountSalesNetCostsPeriodStart, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column55 = DblParser(item.AccountSalesNetCostsDebit, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column56 = DblParser(item.AccountSalesNetCostsCredit, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column57 = DblParser(item.AccountSalesNetCostsPeriodEnd, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column58 = DblParser(item.AccountDiscountsPeriodStart, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column59 = DblParser(item.AccountDiscountsDebit, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column60 = DblParser(item.AccountDiscountsCredit, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column61 = DblParser(item.AccountDiscountsPeriodEnd, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column62 = DblParser(item.AccountValueReductionPeriodStart, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column63 = DblParser(item.AccountValueReductionDebit, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column64 = DblParser(item.AccountValueReductionCredit, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column65 = DblParser(item.AccountValueReductionPeriodEnd, 2)

        Next

        ReportFileName = "R_GoodsTurnoverInfoList.rdlc"
        NumberOfTablesInUse = 1

    End Sub

#End Region

    ''' <summary>
    ''' Map <see cref="ActiveReports.FinancialStatementsInfo">FinancialStatementsInfo</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As ActiveReports.FinancialStatementsInfo, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.FirstPeriodDateStart.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.SecondPeriodDateStart.ToShortDateString
        RD.TableGeneral.Item(0).Column3 = R_Obj.SecondPeriodDateEnd.ToShortDateString
        RD.TableGeneral.Item(0).Column4 = R_Obj.ClosingSummaryAccount.ToString
        RD.TableGeneral.Item(0).Column5 = R_Obj.ClosingSummaryBalanceItem
        RD.TableGeneral.Item(0).Column6 = ConvertDateToWordsLT(R_Obj.SecondPeriodDateEnd)
        RD.TableGeneral.Item(0).Column7 = (Version - 1).ToString


        If Version = 0 OrElse Version = 1 Then

            For Each item As ActiveReports.AccountTurnoverInfo In R_Obj.AccountTurnoverList

                RD.Table1.Rows.Add()

                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.ID.ToString
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.Name
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.FinancialStatementItem
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = DblParser(item.DebitBalanceFormerPeriodStart)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = DblParser(item.CreditBalanceFormerPeriodStart)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = DblParser(item.DebitTurnoverFormerPeriod)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = DblParser(item.CreditTurnoverFormerPeriod)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = DblParser(item.DebitClosingFormerPeriod)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = DblParser(item.CreditClosingFormerPeriod)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = DblParser(item.DebitBalanceCurrentPeriodStart)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = DblParser(item.CreditBalanceCurrentPeriodStart)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(item.DebitTurnoverCurrentPeriod)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = DblParser(item.CreditTurnoverCurrentPeriod)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(item.DebitClosingCurrentPeriod)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = DblParser(item.CreditClosingCurrentPeriod)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = DblParser(item.DebitBalanceCurrentPeriodEnd)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = DblParser(item.CreditBalanceCurrentPeriodEnd)

            Next

        ElseIf Version = 2 Then

            For Each item As ActiveReports.BalanceSheetInfo In R_Obj.BalanceSheet

                RD.Table1.Rows.Add()

                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.Name
                If item.IsCreditBalance Then
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = "X"
                Else
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = ""
                End If
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.Level.ToString
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.RelatedAccounts
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = _
                    item.OptimizedBalanceCurrent.ToString("##,0.00;(##,0.00);""""")
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = _
                    item.OptimizedBalanceFormer.ToString("##,0.00;(##,0.00);""""")
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = _
                    item.ActualBalanceCurrent.ToString("##,0.00;(##,0.00);""""")
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = _
                    item.ActualBalanceFormer.ToString("##,0.00;(##,0.00);""""")
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = item.Number

            Next

        ElseIf Version = 3 Then

            For Each item As ActiveReports.IncomeStatementInfo In R_Obj.IncomeStatement

                RD.Table1.Rows.Add()

                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.Name
                If item.IsCreditBalance Then
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = "X"
                Else
                    RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = ""
                End If
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.Level.ToString
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.RelatedAccounts
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = _
                    item.OptimizedBalanceCurrent.ToString("##,0.00;(##,0.00);""""")
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = _
                    item.OptimizedBalanceFormer.ToString("##,0.00;(##,0.00);""""")
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = _
                    item.ActualBalanceCurrent.ToString("##,0.00;(##,0.00);""""")
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = _
                    item.ActualBalanceFormer.ToString("##,0.00;(##,0.00);""""")
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = item.Number

            Next


        Else

            Throw New NotImplementedException("Klaida. Objekto '" & R_Obj.GetType.FullName & _
                "' versijos " & Version.ToString & " atvaizdavimas ataskaitoje neimplementuotas.")

        End If

        If Version = 0 Then
            ReportFileName = "R_AccountTurnoverInfoList2.rdlc"
        ElseIf Version = 1 Then
            ReportFileName = "R_AccountTurnoverInfoList.rdlc"
        ElseIf Version = 2 Then
            ReportFileName = "R_ConsolidatedReport.rdlc"
        ElseIf Version = 3 Then
            ReportFileName = "R_ConsolidatedReport.rdlc"
        Else
            Throw New NotImplementedException("Klaida. Objekto '" & R_Obj.GetType.FullName & _
                "' versijos " & Version.ToString & " atvaizdavimas ataskaitoje neimplementuotas.")
        End If
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="ActiveReports.DebtInfoList">DebtInfoList</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As ActiveReports.DebtInfoList, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.DateFrom.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.DateTo.ToShortDateString
        RD.TableGeneral.Item(0).Column3 = R_Obj.Account.ToString
        If R_Obj.IsBuyer Then
            RD.TableGeneral.Item(0).Column4 = "X"
            RD.TableGeneral.Item(0).Column5 = ""
        Else
            RD.TableGeneral.Item(0).Column5 = "X"
            RD.TableGeneral.Item(0).Column4 = ""
        End If
        If R_Obj.ShowZeroDebtsFilterState Then
            RD.TableGeneral.Item(0).Column6 = "Taip"
        Else
            RD.TableGeneral.Item(0).Column6 = "Ne"
        End If
        If Not R_Obj.GroupInfo Is Nothing AndAlso R_Obj.GroupInfo.ID > 0 Then
            RD.TableGeneral.Item(0).Column7 = R_Obj.GroupInfo.Name
        Else
            RD.TableGeneral.Item(0).Column7 = "Visos grupės"
        End If

        Dim DebtStart As Double = 0
        Dim TurnoverDebet As Double = 0
        Dim TurnoverCredit As Double = 0
        Dim DebtEnd As Double = 0

        For Each item As ActiveReports.DebtInfo In R_Obj.GetSortedList

            RD.Table1.Rows.Add()

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.PersonID.ToString
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.PersonName
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.PersonCode
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.PersonVatCode
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = item.PersonAddress
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = item.PersonGroup
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = DblParser(item.DebtBegin)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = DblParser(item.TurnoverDebet)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = DblParser(item.TurnoverCredit)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = DblParser(item.DebtEnd)

            DebtStart += item.DebtBegin
            TurnoverDebet += item.TurnoverDebet
            TurnoverCredit += item.TurnoverCredit
            DebtEnd += item.DebtEnd

        Next

        RD.TableGeneral.Item(0).Column8 = DblParser(DebtStart)
        RD.TableGeneral.Item(0).Column9 = DblParser(TurnoverDebet)
        RD.TableGeneral.Item(0).Column10 = DblParser(TurnoverCredit)
        RD.TableGeneral.Item(0).Column11 = DblParser(DebtEnd)

        ReportFileName = "R_DebtInfoList.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="ActiveReports.UnsettledPersonInfoList">UnsettledPersonInfoList</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As ActiveReports.UnsettledPersonInfoList, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.AsOfDate.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.Account.ToString
        RD.TableGeneral.Item(0).Column3 = DblParser(R_Obj.MarginOfError, 2)
        If R_Obj.ForBuyers Then
            RD.TableGeneral.Item(0).Column4 = "X"
            RD.TableGeneral.Item(0).Column5 = ""
            RD.TableGeneral.Item(0).Column6 = "Pirkėjai"
        Else
            RD.TableGeneral.Item(0).Column4 = ""
            RD.TableGeneral.Item(0).Column5 = "X"
            RD.TableGeneral.Item(0).Column6 = "Tiekėjai"
        End If
        If R_Obj.PersonGroup Is Nothing OrElse R_Obj.PersonGroup.IsEmpty Then
            RD.TableGeneral.Item(0).Column7 = "Visos grupės"
        Else
            RD.TableGeneral.Item(0).Column7 = R_Obj.PersonGroup.Name
        End If

        Dim currentPerson As String

        For Each personItem As ActiveReports.UnsettledPersonInfo In R_Obj.GetSortedList

            currentPerson = String.Format("{0} ({1}) skola = {2}", personItem.Name, personItem.Code, _
                DblParser(personItem.Debt, 2))

            For Each item As ActiveReports.UnsettledDocumentInfo In personItem.ItemsSorted

                RD.Table1.Rows.Add()

                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = currentPerson
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.ID.ToString
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.DocTypeHumanReadable
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = item.Date.ToShortDateString
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = item.DocNo
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = item.Content
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = DblParser(item.SumInDocument)
                RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = DblParser(item.Debt)

            Next

        Next

        ReportFileName = "R_UnsettledPersonInfoList.rdlc"
        NumberOfTablesInUse = 1

    End Sub

    ''' <summary>
    ''' Map <see cref="ActiveReports.ServiceTurnoverInfoList">ServiceTurnoverInfoList</see> to 
    ''' <see cref="AccControls.ReportData">ReportData</see> dataset.
    ''' </summary>
    ''' <param name="RD">Report dataset of type AccControls.ReportData.</param>
    ''' <param name="R_Obj">Object to be maped.</param>
    ''' <param name="ReportFileName">.rdlc form file name.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tables needed to map object data.</param>
    ''' <param name="Version">Version of .rdlc form.</param>
    Friend Sub MapObjectDetailsToReport(ByRef RD As AccControls.ReportData, _
        ByVal R_Obj As ActiveReports.ServiceTurnoverInfoList, ByRef ReportFileName As String, _
        ByRef NumberOfTablesInUse As Integer, ByVal Version As Integer)

        RD.TableGeneral.Item(0).Column1 = R_Obj.DateFrom.ToShortDateString
        RD.TableGeneral.Item(0).Column2 = R_Obj.DateTo.ToShortDateString
        RD.TableGeneral.Item(0).Column3 = R_Obj.TradedType
        If R_Obj.ShowWithoutTurnover Then
            RD.TableGeneral.Item(0).Column4 = "Taip"
        Else
            RD.TableGeneral.Item(0).Column4 = "Ne"
        End If

        Dim purchasedSum As Double = 0
        Dim purchasedSumReturned As Double = 0
        Dim purchasedSumReductions As Double = 0
        Dim soldSum As Double = 0
        Dim soldSumReturned As Double = 0
        Dim soldSumReductions As Double = 0
        Dim discounts As Double = 0

        For Each item As ActiveReports.ServiceTurnoverInfo In R_Obj.GetSortedList

            RD.Table1.Rows.Add()

            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column1 = item.ID.ToString()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column2 = item.Name
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column3 = item.TradedType
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column4 = DblParser(item.DefaultRateVatSales, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column5 = DblParser(item.DefaultRateVatPurchase, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column6 = item.ServiceCode
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column7 = BooleanToCheckMark(item.IsObsolete)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column8 = item.AccountSales.ToString()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column9 = item.AccountPurchase.ToString()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column10 = item.AccountVatPurchase.ToString()
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column11 = DblParser(item.PurchasedAmount, ROUNDAMOUNTINVOICERECEIVED)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column12 = DblParser(item.PurchasedSum, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column13 = DblParser(item.PurchasedAmountReturned, ROUNDAMOUNTINVOICERECEIVED)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column14 = DblParser(item.PurchasedSumReturned, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column15 = DblParser(item.PurchasedSumReductions, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column16 = DblParser(item.SoldAmount, ROUNDAMOUNTINVOICEMADE)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column17 = DblParser(item.SoldSum, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column18 = DblParser(item.SoldAmountReturned, ROUNDAMOUNTINVOICEMADE)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column19 = DblParser(item.SoldSumReturned, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column20 = DblParser(item.SoldSumReductions, 2)
            RD.Table1.Item(RD.Table1.Rows.Count - 1).Column21 = DblParser(item.SoldSumDiscounts, 2)

            purchasedSum = CRound(purchasedSum + item.PurchasedSum, 2)
            purchasedSumReturned = CRound(purchasedSumReturned + item.PurchasedSumReturned, 2)
            purchasedSumReductions = CRound(purchasedSumReductions + item.PurchasedSumReductions, 2)
            soldSum = CRound(soldSum + item.SoldSum, 2)
            soldSumReturned = CRound(soldSumReturned + item.SoldSumReturned, 2)
            soldSumReductions = CRound(soldSumReductions + item.SoldSumReductions, 2)
            discounts = CRound(discounts + item.SoldSumDiscounts, 2)

        Next

        RD.TableGeneral.Item(0).Column5 = DblParser(purchasedSum, 2)
        RD.TableGeneral.Item(0).Column6 = DblParser(purchasedSumReturned, 2)
        RD.TableGeneral.Item(0).Column7 = DblParser(purchasedSumReductions, 2)
        RD.TableGeneral.Item(0).Column8 = DblParser(soldSum, 2)
        RD.TableGeneral.Item(0).Column9 = DblParser(soldSumReturned, 2)
        RD.TableGeneral.Item(0).Column10 = DblParser(soldSumReductions, 2)
        RD.TableGeneral.Item(0).Column11 = DblParser(discounts, 2)

        ReportFileName = "R_ServiceTurnoverInfoList.rdlc"
        NumberOfTablesInUse = 1

    End Sub

#End Region

#Region "Helpers"

    Private Function BooleanToCheckMark(ByVal cValue As Boolean) As String
        If cValue Then Return "X"
        Return ""
    End Function

    Private Sub CopyDataTableValues(ByVal SourceDataTable As DataTable, ByRef TargetDataTable As DataTable)
        Dim i As Integer
        For Each dr As DataRow In SourceDataTable.Rows
            TargetDataTable.Rows.Add()
            For i = 1 To SourceDataTable.Columns.Count
                TargetDataTable.Rows(TargetDataTable.Rows.Count - 1).Item(i - 1) = dr.Item(i - 1)
            Next
        Next
    End Sub

    Private Sub CopyDataTable(ByRef TargetDataTable As DataTable, _
        ByVal SourceDataSet As DataSet, ByVal SourceTableName As String)
        Dim i, j As Integer
        For i = 1 To SourceDataSet.Tables(SourceTableName).Rows.Count
            TargetDataTable.Rows.Add()
            For j = 1 To Math.Min(SourceDataSet.Tables(SourceTableName).Columns.Count, _
                TargetDataTable.Columns.Count)

                TargetDataTable.Rows(i - 1).Item(j - 1) = _
                    SourceDataSet.Tables(SourceTableName).Rows(i - 1).Item(j - 1)

            Next
        Next
    End Sub

    Private Function ConvertDateToWordsLT(ByVal DateToConvert As Date) As String
        Select Case DateToConvert.Month
            Case 1
                Return DateToConvert.Year.ToString & " m. Sausio mėn. " & DateToConvert.Day.ToString & " d."
            Case 2
                Return DateToConvert.Year.ToString & " m. Vasario mėn. " & DateToConvert.Day.ToString & " d."
            Case 3
                Return DateToConvert.Year.ToString & " m. Kovo mėn. " & DateToConvert.Day.ToString & " d."
            Case 4
                Return DateToConvert.Year.ToString & " m. Balandžio mėn. " & DateToConvert.Day.ToString & " d."
            Case 5
                Return DateToConvert.Year.ToString & " m. Gegužės mėn. " & DateToConvert.Day.ToString & " d."
            Case 6
                Return DateToConvert.Year.ToString & " m. Birželio mėn. " & DateToConvert.Day.ToString & " d."
            Case 7
                Return DateToConvert.Year.ToString & " m. Liepos mėn. " & DateToConvert.Day.ToString & " d."
            Case 8
                Return DateToConvert.Year.ToString & " m. Rugpjūčio mėn. " & DateToConvert.Day.ToString & " d."
            Case 9
                Return DateToConvert.Year.ToString & " m. Rugsėjo mėn. " & DateToConvert.Day.ToString & " d."
            Case 10
                Return DateToConvert.Year.ToString & " m. Spalio mėn. " & DateToConvert.Day.ToString & " d."
            Case 11
                Return DateToConvert.Year.ToString & " m. Lapkričio mėn. " & DateToConvert.Day.ToString & " d."
            Case 12
                Return DateToConvert.Year.ToString & " m. Gruodžio mėn. " & DateToConvert.Day.ToString & " d."
            Case Else
                Throw New ArgumentOutOfRangeException("Invalid date " & DateToConvert.ToShortDateString & ".")
        End Select
    End Function

    Private Function SplitStringByMaxLength(ByVal StringToSplit As String, _
        ByVal MaxCharCountInLine As Integer) As String()

        If StringToSplit Is Nothing OrElse String.IsNullOrEmpty(StringToSplit.Trim) Then _
            Return New String() {""}

        If Not StringToSplit.Trim.Length > MaxCharCountInLine Then _
            Return New String() {StringToSplit.Trim}

        Dim result As New List(Of String)

        Dim WordString As String = ""

        For Each c As Char In StringToSplit

            WordString = WordString & c

            If c = ","c OrElse c = "."c OrElse c = ";"c OrElse c = " "c _
                OrElse c = "-"c OrElse c = "!"c OrElse c = "?"c OrElse c = "%"c _
                OrElse c = "&"c OrElse c = "%"c OrElse c = "+"c Then

                If Not String.IsNullOrEmpty(WordString.Trim) Then result.Add(WordString.Trim)
                WordString = ""

            End If

        Next

        If Not String.IsNullOrEmpty(WordString.Trim) Then result.Add(WordString.Trim)
        WordString = ""

        Dim FinalResult As New List(Of String)

        For Each w As String In result

            If String.IsNullOrEmpty(WordString.Trim) Then
                WordString = w
            ElseIf Not (WordString.Trim & " " & w).Length > MaxCharCountInLine Then
                WordString = WordString & " " & w
            Else
                FinalResult.Add(WordString.Trim)
                WordString = w
            End If

        Next

        If Not String.IsNullOrEmpty(WordString.Trim) Then FinalResult.Add(WordString.Trim)

        Return FinalResult.ToArray

    End Function

#End Region

End Module