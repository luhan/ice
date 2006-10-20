# **********************************************************************
#
# Copyright (c) 2003-2006 ZeroC, Inc. All rights reserved.
#
# This copy of Ice is licensed to you under the terms described in the
# ICE_LICENSE file included in this distribution.
#
# **********************************************************************

top_srcdir	= ..\..

PKG		= icestormcs
LIBNAME		= $(PKG).dll
TARGETS		= $(bindir)\$(LIBNAME)

SRCS		= AssemblyInfo.cs

SLICE_SRCS	= $(SDIR)\IceStorm.ice

GEN_SRCS	= $(GDIR)\IceStorm.cs

SDIR		= $(slicedir)\IceStorm
GDIR		= generated

!include $(top_srcdir)\config\Make.rules.mak

MCSFLAGS	= $(MCSFLAGS) -target:library -out:$(TARGETS) -unsafe
!if "$(DOTNET_1)" != "yes"
MCSFLAGS	= $(MCSFLAGS) -keyfile:$(top_srcdir)\config\IcecsKey.snk
!endif

SLICE2CSFLAGS	= $(SLICE2CSFLAGS) -I$(slicedir) --ice

$(TARGETS):: $(SRCS) $(GEN_SRCS)
	$(MCS) $(MCSFLAGS) -r:$(bindir)\icecs.dll $(SRCS) $(GEN_SRCS)

!if "$(NOGAC)" == ""

install:: all
	$(GACUTIL) -i $(bindir)\$(LIBNAME)

!else

install:: all
	copy $(bindir)\icestormcs.dll $(install_bindir)

!endif

!include .depend
