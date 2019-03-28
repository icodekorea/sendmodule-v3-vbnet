Imports System.Net.Sockets

Public Class ICodeSMS
    Public ServerIPAddress As String    '서버IP
    Public ServerPort As String         '접속Port
    Public Token As String              '토큰키

    Public ResultMsg As SMSLog
    Public Delegate Sub SMSLog(msg As String)

    Dim rCodes As New Dictionary(Of String, String)

    Public Sub New()
        ' 발송 결과 코드 배열 정리
        rCodes.Add("00", "발송성공")
        rCodes.Add("17", "물량이 많은 관계로 대기처리")
        rCodes.Add("23", "데이터 형식오류, 전송날짜 오류")
        rCodes.Add("79", "소켓접속 가능횟수 초과. 소켓 Close 필요")
        rCodes.Add("85", "미등록된 발신번호. 발신번호 등록 필요")
        rCodes.Add("87", "발송인증 실패. 문의확인")
        rCodes.Add("88", "소켓모듈 사용권한 없음")
        rCodes.Add("96", "토큰키 사용 불가")
        rCodes.Add("97", "잔여코인 부족")
        rCodes.Add("98", "사용기간 만료")
        rCodes.Add("99", "인증실패")
    End Sub

    ' 발송처리 함수 (발신번호, 수신번호, 메시지, 예약일자, 제목)
    ' 제목은 장문만 지원되며, 폰에서 지원되어야 발송 가능함.
    Public Sub SendSocket(cb As String, tel As String, msg As String, rdate As String, title As String)
        '엔터값 치환.
        msg = msg.Replace(vbCr, "")
        msg = msg.Replace(vbLf, "\n")

        '패킷구성
        Dim packetMsg As String = "{" +
            """key"":""" + Token + """," +
            """tel"":""" + tel + """," +
            """cb"":""" + cb + """," +
            """msg"":""" + msg + """," +
            """title"":""" + title + """," +
            """date"":""" + rdate + """," +
            """charset"":""euc-kr""}" 'euc-kr 타입으로 패킷발송. utf-8인 경우 변경.
        Dim splen As Integer = System.Text.Encoding.Default.GetByteCount(packetMsg)
        packetMsg = String.Format("06{0:D4}{1}", splen, packetMsg)
        ResultMsg(packetMsg)

        ' Client 소켓 생성
        Dim clientSocket As New TcpClient()
        Try
            clientSocket.Connect(ServerIPAddress, ServerPort)
            ResultMsg("connected Socket.")

            '구성된 패킷 서버로 발송
            Dim serverStream As NetworkStream = clientSocket.GetStream()
            Dim outStream As Byte() = System.Text.Encoding.Default.GetBytes(packetMsg)
            serverStream.Write(outStream, 0, outStream.Length)
            serverStream.Flush()

            '서버로 부터 결과 코드 수신
            Dim bufflen As Integer = 0
            Dim inStream(31) As Byte
            Dim returndata As String = ""
            While (bufflen < 31)
                Dim len As Integer = serverStream.Read(inStream, 0, inStream.Length)
                returndata += System.Text.Encoding.Default.GetString(inStream).Substring(0, len)
                bufflen += len
            End While
            '수신받은 결과 정보
            ResultMsg("recv: " + returndata)

            '결과코드에 따른 설명 표기
            Dim resultCode As String = returndata.Substring(6, 2).Trim()
            ResultMsg("결과: " + resultCode + ")" + rCodes.Item(resultCode))
        Catch ex As Exception
        End Try

        Try
            clientSocket.Close()
            ResultMsg("Socket Close")
        Catch ex As Exception

        End Try
    End Sub

End Class
