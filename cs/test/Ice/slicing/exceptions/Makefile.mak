# **********************************************************************
#
# Copyright (c) 2003-2006 ZeroC, Inc. All rights reserved.
#
# This copy of Ice is licensed to you under the terms described in the
# ICE_LICENSE file included in this distribution.
#
# **********************************************************************

top_srcdir	= ..\..\..\..

TARGETS		= client.exe server.exe serveramd.exe

C_SRCS		= AllTests.cs Client.cs
S_SRCS		= Server.cs TestI.cs
SAMD_SRCS	= Server.cs TestAMDI.cs

SLICE_SRCS	= $(SDIR)\Test.ice
SLICE_S_SRCS	= $(SDIR)\ServerPrivate.ice
SLICE_AMD_SRCS	= $(SDIR)\TestAMD.ice
SLICE_SAMD_SRCS	= $(SDIR)\ServerPrivateAMD.ice

GEN_SRCS	= $(GDIR)\Test.cs
SGEN_SRCS	= $(GDIR)\ServerPrivate.cs
GEN_AMD_SRCS	= $(GDIR)\TestAMD.cs
SAMD_GEN_SRCS	= $(GDIR)\ServerPrivateAMD.cs

SDIR		= .

GDIR		= generated

!include $(top_srcdir)\config\Make.rules.mak

MCSFLAGS	= $(MCSFLAGS) -target:exe

SLICE2CSFLAGS	= $(SLICE2CSFLAGS) -I.

client.exe: $(C_SRCS) $(GEN_SRCS)
	$(MCS) $(MCSFLAGS) -out:$@ -r:$(bindir)\icecs.dll $(C_SRCS) $(GEN_SRCS)

server.exe: $(S_SRCS) $(GEN_SRCS) $(SGEN_SRCS)
	$(MCS) $(MCSFLAGS) -out:$@ -r:$(bindir)\icecs.dll $(S_SRCS) $(GEN_SRCS) $(SGEN_SRCS)

serveramd.exe: $(SAMD_SRCS) $(GEN_AMD_SRCS) $(SAMD_GEN_SRCS)
	$(MCS) $(MCSFLAGS) -out:$@ -r:$(bindir)\icecs.dll $(SAMD_SRCS) $(GEN_AMD_SRCS) $(SAMD_GEN_SRCS)

!include .depend
