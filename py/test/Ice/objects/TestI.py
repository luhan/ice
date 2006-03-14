#!/usr/bin/env python
# **********************************************************************
#
# Copyright (c) 2003-2006 ZeroC, Inc. All rights reserved.
#
# This copy of Ice is licensed to you under the terms described in the
# ICE_LICENSE file included in this distribution.
#
# **********************************************************************

import Ice, Test

class BI(Test.B):
    def __init__(self):
        self.preMarshalInvoked = False
        self._postUnmarshalInvoked = False

    def postUnmarshalInvoked(self, current=None):
        return self._postUnmarshalInvoked

    def ice_preMarshal(self):
        self.preMarshalInvoked = True

    def ice_postUnmarshal(self):
        self._postUnmarshalInvoked = True

class CI(Test.C):
    def __init__(self):
        self.preMarshalInvoked = False
        self._postUnmarshalInvoked = False

    def postUnmarshalInvoked(self, current=None):
        return self._postUnmarshalInvoked

    def ice_preMarshal(self):
        self.preMarshalInvoked = True

    def ice_postUnmarshal(self):
        self._postUnmarshalInvoked = True

class DI(Test.D):
    def __init__(self):
        self.preMarshalInvoked = False
        self._postUnmarshalInvoked = False

    def postUnmarshalInvoked(self, current=None):
        return self._postUnmarshalInvoked

    def ice_preMarshal(self):
        self.preMarshalInvoked = True

    def ice_postUnmarshal(self):
        self._postUnmarshalInvoked = True

class InitialI(Test.Initial):
    def __init__(self, adapter):
        self._adapter = adapter
        self._b1 = BI()
        self._b2 = BI()
        self._c = CI()
        self._d = DI()

        self._b1.theA = self._b2 # Cyclic reference to another B
        self._b1.theB = self._b1 # Self reference.
        self._b1.theC = None     # Null reference.

        self._b2.theA = self._b2 # Self reference, using base.
        self._b2.theB = self._b1 # Cyclic reference to another B
        self._b2.theC = self._c  # Cyclic reference to a C.

        self._c.theB = self._b2  # Cyclic reference to a B.

        self._d.theA = self._b1  # Reference to a B.
        self._d.theB = self._b2  # Reference to a B.
        self._d.theC = None      # Reference to a C.

    def shutdown(self, current=None):
        self._adapter.getCommunicator().shutdown()

    def getB1(self, current=None):
        self._b1.preMarshalInvoked = False
        self._b2.preMarshalInvoked = False
        self._c.preMarshalInvoked = False
        return self._b1

    def getB2(self, current=None):
        self._b1.preMarshalInvoked = False
        self._b2.preMarshalInvoked = False
        self._c.preMarshalInvoked = False
        return self._b2

    def getC(self, current=None):
        self._b1.preMarshalInvoked = False
        self._b2.preMarshalInvoked = False
        self._c.preMarshalInvoked = False
        return self._c

    def getD(self, current=None):
        self._b1.preMarshalInvoked = False
        self._b2.preMarshalInvoked = False
        self._c.preMarshalInvoked = False
        self._d.preMarshalInvoked = False
        return self._d

    def getAll(self, current=None):
        self._b1.preMarshalInvoked = False
        self._b2.preMarshalInvoked = False
        self._c.preMarshalInvoked = False
        self._d.preMarshalInvoked = False
        return (self._b1, self._b2, self._c, self._d)
