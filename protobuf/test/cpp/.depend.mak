Test$(OBJEXT): Test.cpp Test.h Test.pb.h StreamProtobuf.h
Client$(OBJEXT): Client.cpp ../../../cpp/test/include/TestCommon.h Test.h Test.pb.h StreamProtobuf.h
AllTests$(OBJEXT): AllTests.cpp ../../../cpp/test/include/TestCommon.h Test.h Test.pb.h StreamProtobuf.h
Test.pb$(OBJEXT): Test.pb.cpp Test.pb.h
Test$(OBJEXT): Test.cpp Test.h Test.pb.h StreamProtobuf.h
TestI$(OBJEXT): TestI.cpp TestI.h Test.h Test.pb.h StreamProtobuf.h
Server$(OBJEXT): Server.cpp TestI.h Test.h Test.pb.h StreamProtobuf.h
Test.pb$(OBJEXT): Test.pb.cpp Test.pb.h
