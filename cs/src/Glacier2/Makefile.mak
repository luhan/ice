# **********************************************************************
#
# Copyright (c) 2003-2006 ZeroC, Inc. All rights reserved.
#
# This copy of Ice is licensed to you under the terms described in the
# ICE_LICENSE file included in this distribution.
#
# **********************************************************************

top_srcdir	= ..\..

PKG		= glacier2cs
LIBNAME		= $(PKG).dll
TARGETS		= $(bindir)\$(LIBNAME)

SRCS		= AssemblyInfo.cs

SLICE_SRCS	= $(SDIR)\PermissionsVerifier.ice \
		  $(SDIR)\PermissionsVerifierF.ice \
		  $(SDIR)\Router.ice \
		  $(SDIR)\RouterF.ice \
		  $(SDIR)\Session.ice \
		  $(SDIR)\SessionF.ice \
		  $(SDIR)\SSLInfo.ice

GEN_SRCS	= $(GDIR)\PermissionsVerifier.cs \
		  $(GDIR)\PermissionsVerifierF.cs \
		  $(GDIR)\Router.cs \
		  $(GDIR)\RouterF.cs \
		  $(GDIR)\Session.cs \
		  $(GDIR)\SessionF.cs \
		  $(GDIR)\SSLInfo.cs

SDIR		= $(slicedir)\Glacier2
GDIR		= generated

!include $(top_srcdir)/config/Make.rules.mak

MCSFLAGS	= $(MCSFLAGS) -target:library -out:$(TARGETS) -unsafe -warnaserror-
!if "$(DOTNET_1)" != "yes"
MCSFLAGS	= $(MCSFLAGS) -keyfile:$(top_srcdir)\config\IcecsKey.snk
!endif

SLICE2CSFLAGS	= $(SLICE2CSFLAGS) -I$(slicedir)

$(TARGETS):: $(SRCS) $(GEN_SRCS)
	$(MCS) $(MCSFLAGS) -r:$(bindir)\icecs.dll $(SRCS) $(GEN_SRCS)

!if "$(NOGAC)" == ""

install:: all
	$(GACUTIL) -i $(bindir)\$(LIBNAME)

!else

install:: all
	copy $(bindir)\glacier2cs.dll $(install_bindir)

!endif

!include .depend
