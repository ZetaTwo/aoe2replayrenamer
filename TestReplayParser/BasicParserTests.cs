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
        public void TestRecFloatCompare()
        {
            Aoe2ReplayHeaderBasic header = Aoe2RecordParser.BasicParse("recs-extra\\de-float.aoe2record");
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
        public void TestRecAiNames()
        {
            Aoe2ReplayHeaderBasic header = Aoe2RecordParser.BasicParse("recs-extra\\de-ai-names.aoe2record");
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
        public void TestRecAiFfffffff()
        {
            Aoe2ReplayHeaderBasic header = Aoe2RecordParser.BasicParse("recs-extra\\de-ai-ffffffff.aoe2record");
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
