Imports System.Net.Sockets

Class MainWindow

    Dim socketSMS As ICodeSMS '발송객체

    ' 폼 호출시 ICodeSMS 객체 생성 및 초기설정.
    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        socketSMS = New ICodeSMS
        socketSMS.ResultMsg = AddressOf msg ' 로그출력 함수 전달
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        '서버정보 전달
        socketSMS.ServerIPAddress = txtServerIP.Text
        socketSMS.ServerPort = txtServerPort.Text
        '토큰키 전달
        socketSMS.Token = txtTokenkey.Text

        '문자발송호출
        socketSMS.SendSocket(txtCallback.Text, txtPhoneNumber.Text, txtSendMessage.Text, txtDateTime.Text, txtTitle.Text)
    End Sub

    ' 로그 출력함수 구현.
    Sub msg(msg As String)
        logBox.Dispatcher.Invoke(Sub() logBox.AppendText(msg + Chr(13)))
        logBox.Dispatcher.Invoke(Sub() logBox.ScrollToEnd())
    End Sub

    '문구타입 및 길이 계산
    Private Sub txtSendMessage_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtSendMessage.TextChanged
        Dim msg As String = txtSendMessage.Text.Replace(vbCr, "")
        Dim splen As Integer = System.Text.Encoding.Default.GetByteCount(msg)
        If (lblSendLength IsNot Nothing And lblSendType IsNot Nothing) Then
            If (splen > 90) Then lblSendType.Content = "LMS" Else lblSendType.Content = "SMS"
            lblSendLength.Content = splen.ToString() + "b"
        End If
    End Sub
End Class
