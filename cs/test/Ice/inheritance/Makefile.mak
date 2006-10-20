# **********************************************************************
#
# Copyright (c) 2003-2006 ZeroC, Inc. All rights reserved.
#
# This copy of Ice is licensed to you under the terms described in the
# ICE_LICENSE file included in this distribution.
#
# **********************************************************************

top_srcdir	= ..\..\..

TARGETS		= client.exe server.exe collocated.exe

C_SRCS		= AllTests.cs Client.cs
S_SRCS		= CAI.cs CBI.cs CCI.cs CDI.cs IAI.cs IB1I.cs IB2I.cs ICI.cs InitialI.cs Server.cs
COL_SRCS	= AllTests.cs CAI.cs CBI.cs CCI.cs CDI.cs Collocated.cs IAI.cs IB1I.cs IB2I.cs ICI.cs InitialI.cs

SLICE_SRCS	= $(SDIR)\Test.ice

GEN_SRCS	= $(GDIR)\Test.cs

SDIR		= .

GDIR		= generated

!include $(top_srcdir)\config\Make.rules.mak

MCSFLAGS	= $(MCSFLAGS) -target:exe

SLICE2CSFLAGS	= $(SLICE2CSFLAGS) -I.

client.exe: $(C_SRCS) $(GEN_SRCS)
	$(MCS) $(MCSFLAGS) -out:$@ -r:$(bindir)\icecs.dll $(C_SRCS) $(GEN_SRCS)

server.exe: $(S_SRCS) $(GEN_SRCS)
	$(MCS) $(MCSFLAGS) -out:$@ -r:$(bindir)\icecs.dll $(S_SRCS) $(GEN_SRCS)

collocated.exe: $(COL_SRCS) $(GEN_SRCS)
	$(MCS) $(MCSFLAGS) -out:$@ -r:$(bindir)\icecs.dll $(COL_SRCS) $(GEN_SRCS)

!include .depend
