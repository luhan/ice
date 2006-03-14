#!/usr/bin/env python
# **********************************************************************
#
# Copyright (c) 2003-2006 ZeroC, Inc. All rights reserved.
#
# This copy of Ice is licensed to you under the terms described in the
# ICE_LICENSE file included in this distribution.
#
# **********************************************************************

import sys, traceback, Ice

Ice.loadSlice('Callback.ice')
import Demo

class CallbackI(Demo.Callback):
    def initiateCallback(self, proxy, current=None):
        print "initiating callback to: " + current.adapter.getCommunicator().proxyToString(proxy)
	try:
	    proxy.callback(current.ctx)
	except:
	    traceback.print_exc()

    def shutdown(self, current=None):
        print "shutting down..."
        current.adapter.getCommunicator().shutdown()

class Server(Ice.Application):
    def run(self, args):
	adapter = self.communicator().createObjectAdapter("Callback.Server")
	adapter.add(CallbackI(), Ice.stringToIdentity("callback"))
	adapter.activate()
	self.communicator().waitForShutdown()
	return True

app = Server()
sys.exit(app.main(sys.argv, "config.server"))
