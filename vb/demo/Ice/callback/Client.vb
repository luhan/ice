' **********************************************************************
'
' Copyright (c) 2003-2006 ZeroC, Inc. All rights reserved.
'
' This copy of Ice is licensed to you under the terms described in the
' ICE_LICENSE file included in this distribution.
'
' **********************************************************************
Imports System
Imports Demo

Module CallbackC
    Class Client
        Inherits Ice.Application

        Private Sub menu()
            Console.Out.WriteLine("usage:")
            Console.Out.WriteLine("t: send callback as twoway")
            Console.Out.WriteLine("o: send callback as oneway")
            Console.Out.WriteLine("O: send callback as batch oneway")
            Console.Out.WriteLine("d: send callback as datagram")
            Console.Out.WriteLine("D: send callback as batch datagram")
            Console.Out.WriteLine("f: flush all batch requests")
            Console.Out.WriteLine("s: shutdown server")
            Console.Out.WriteLine("x: exit")
            Console.Out.WriteLine("?: help")
        End Sub

        Public Overloads Overrides Function run(ByVal args() As String) As Integer
            Dim properties As Ice.Properties = communicator().getProperties()
            Dim proxyProperty As String = "Callback.Client.CallbackServer"
            Dim proxy As String = properties.getProperty(proxyProperty)
            If proxy.Length = 0 Then
                Console.Error.WriteLine("property `" & proxyProperty & "' not set")
                Return 1
            End If

            Dim twoway As CallbackSenderPrx = CallbackSenderPrxHelper.checkedCast(communicator().stringToProxy(proxy).ice_twoway().ice_timeout(-1).ice_secure(False))
            If twoway Is Nothing Then
                Console.Error.WriteLine("invalid proxy")
                Return 1
            End If
            Dim oneway As CallbackSenderPrx = CallbackSenderPrxHelper.uncheckedCast(twoway.ice_oneway())
            Dim batchOneway As CallbackSenderPrx = CallbackSenderPrxHelper.uncheckedCast(twoway.ice_batchOneway())
            Dim datagram As CallbackSenderPrx = CallbackSenderPrxHelper.uncheckedCast(twoway.ice_datagram())
            Dim batchDatagram As CallbackSenderPrx = CallbackSenderPrxHelper.uncheckedCast(twoway.ice_batchDatagram())

            Dim adapter As Ice.ObjectAdapter = communicator().createObjectAdapter("Callback.Client")
            adapter.add(New CallbackReceiverI, Ice.Util.stringToIdentity("callbackReceiver"))
            adapter.activate()

            Dim twowayR As CallbackReceiverPrx = CallbackReceiverPrxHelper.uncheckedCast( _
                adapter.createProxy(Ice.Util.stringToIdentity("callbackReceiver")))
            Dim onewayR As CallbackReceiverPrx = CallbackReceiverPrxHelper.uncheckedCast( _
                twowayR.ice_oneway())
            Dim datagramR As CallbackReceiverPrx = CallbackReceiverPrxHelper.uncheckedCast(twowayR.ice_datagram())

            menu()

            Dim line As String = Nothing
            Do
                Try
                    Console.Out.Write("==> ")
                    Console.Out.Flush()
                    line = Console.In.ReadLine()
                    If line Is Nothing Then
                        Exit Try
                    End If
                    If line.Equals("t") Then
                        twoway.initiateCallback(twowayR)
                    ElseIf line.Equals("o") Then
                        oneway.initiateCallback(onewayR)
                    ElseIf line.Equals("O") Then
                        batchOneway.initiateCallback(onewayR)
                    ElseIf line.Equals("d") Then
                        datagram.initiateCallback(datagramR)
                    ElseIf line.Equals("D") Then
                        batchDatagram.initiateCallback(datagramR)
                    ElseIf line.Equals("f") Then
                        communicator().flushBatchRequests()
                    ElseIf line.Equals("s") Then
                        twoway.shutdown()
                    ElseIf line.Equals("x") Then
                        '  Nothing to do
                    ElseIf line.Equals("?") Then
                        menu()
                    Else
                        Console.Out.WriteLine("unknown command `" & line & "'")
                        menu()
                    End If
                Catch ex As System.Exception
                    Console.Error.WriteLine(ex)
                End Try
            Loop While Not line.Equals("x")

            Return 0
        End Function

    End Class

    Public Sub Main(ByVal args() As String)
        Dim app As Client = New Client
        Dim status As Integer = app.main(args, "config")
        System.Environment.Exit(status)
    End Sub

End Module
