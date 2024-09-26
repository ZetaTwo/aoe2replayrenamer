using Aoe2BasicReplayParser;

namespace TestReplayParser
{
    [TestClass]
    public class BasicParserTests
    {
        [TestMethod]
        public void TestRec62_0()
        {
            Aoe2ReplayHeaderBasic header = Aoe2RecordParser.BasicParse("recs/de-62.0.aoe2record");
            Assert.AreEqual(header.Datetime, new DateTime(638570863110000000));
            Assert.AreEqual(2, header.Players.Count);

            Assert.AreEqual(1, header.Players[0].PlayerNumber);
            Assert.AreEqual("_LHD_xiaohai", header.Players[0].Name);
            Assert.AreEqual(2, header.Players[0].TeamId);

            Assert.AreEqual(2, header.Players[1].PlayerNumber);
            Assert.AreEqual("Mars_zZ", header.Players[1].Name);
            Assert.AreEqual(3, header.Players[1].TeamId);
        }

        [TestMethod]
        public void TestRec61_5()
        {
            Aoe2ReplayHeaderBasic header = Aoe2RecordParser.BasicParse("recs/de-61.5.aoe2record");
            Assert.AreEqual(header.Datetime, new DateTime(638460425710000000));
            Assert.AreEqual(2, header.Players.Count);

            Assert.AreEqual(1, header.Players[0].PlayerNumber);
            Assert.AreEqual("Hearttt", header.Players[0].Name);
            Assert.AreEqual(2, header.Players[0].TeamId);

            Assert.AreEqual(2, header.Players[1].PlayerNumber);
            Assert.AreEqual("GL.Hera", header.Players[1].Name);
            Assert.AreEqual(3, header.Players[1].TeamId);
        }

        [TestMethod]
        public void TestRecOld1()
        {
            Aoe2ReplayHeaderBasic header = Aoe2RecordParser.BasicParse("C:\\Users\\calle\\Games\\Age of Empires 2 DE\\76561198009420968\\savegame\\MP Replay v101.101.56005.0 @2021.12.25 004616 (2).aoe2record");
            Assert.IsNull(header.Datetime);
            Assert.AreEqual(8, header.Players.Count);

            Assert.AreEqual(1, header.Players[0].PlayerNumber);
            Assert.AreEqual("Torgblottaren", header.Players[0].Name);
            Assert.AreEqual(2, header.Players[0].TeamId);

            Assert.AreEqual(2, header.Players[1].PlayerNumber);
            Assert.AreEqual("Zeta Two", header.Players[1].Name);
            Assert.AreEqual(2, header.Players[1].TeamId);
        }

        [TestMethod]
        public void TestRecOld2()
        {
            Aoe2ReplayHeaderBasic header = Aoe2RecordParser.BasicParse("C:\\Users\\calle\\Games\\Age of Empires 2 DE\\76561198009420968\\savegame\\MP Replay v101.102.28520.0 @2023.09.25 235108 (1).aoe2record");
            Assert.IsNull(header.Datetime);
            Assert.AreEqual(2, header.Players.Count);

            Assert.AreEqual(1, header.Players[0].PlayerNumber);
            Assert.AreEqual("Zeta Two", header.Players[0].Name);
            Assert.AreEqual(2, header.Players[0].TeamId);

            Assert.AreEqual(2, header.Players[1].PlayerNumber);
            Assert.AreEqual("Torgblottaren", header.Players[1].Name);
            Assert.AreEqual(3, header.Players[1].TeamId);
        }


        [TestMethod]
        public void TestRecOld3()
        {
            Aoe2ReplayHeaderBasic header = Aoe2RecordParser.BasicParse("C:\\Users\\calle\\Games\\Age of Empires 2 DE\\76561198009420968\\savegame\\MP Replay v101.102.46236.0 @2024.05.03 214225 (1).aoe2record");
            Assert.AreEqual(header.Datetime, new DateTime(638503585450000000));
            Assert.AreEqual(3, header.Players.Count);

            Assert.AreEqual(1, header.Players[0].PlayerNumber);
            Assert.AreEqual("Zeta Two", header.Players[0].Name);
            Assert.AreEqual(1, header.Players[0].TeamId);

            Assert.AreEqual(1, header.Players[1].PlayerNumber);
            Assert.AreEqual("bgm", header.Players[1].Name);
            Assert.AreEqual(1, header.Players[1].TeamId);
        }
    }
}
