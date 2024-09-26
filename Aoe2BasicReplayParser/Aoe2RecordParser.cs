using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Aoe2BasicReplayParser
{
    public enum GameType
    {
        RM = 0,
        Regicide = 1,
        DM = 2,
        Scenario = 3,
        Campaign = 4,
        KingOfTheHill = 5,
        WonderRace = 6,
        DefendTheWonder = 7,
        TurboRandom = 8,
        CaptureTheRelic = 10,
        SuddenDeath = 11,
        BattleRoyale = 12,
        EmpireWars = 13,
        CoOpCampaign = 15,
    }

    public record Aoe2ReplayPlayerBasic
    {
        public required int PlayerNumber { get; init; }
        public required string Name { get; init; }


        public required int TeamId { get; init; }
    }

    public record Aoe2ReplayHeaderBasic
    {
        public DateTime? Datetime { get; init; }
        public required GameType GameType { get; init; }
        public required List<Aoe2ReplayPlayerBasic> Players { get; init; }

        public string[][] GetTeams()
        {
            Dictionary<int, List<string>> teams = new Dictionary<int, List<string>>();
            foreach (var player in Players)
            {
                if (player.PlayerNumber == -1)
                {
                    continue;
                }
                teams.TryAdd(player.TeamId, new List<string>());
                teams[player.TeamId].Add(player.Name);
            }
            return teams.Values.Select(t => t.ToArray()).ToArray();
        }
    }

    public class Aoe2RecordParser
    {
        private static readonly byte[] MARKER_CONST1 = [0xA3, 0x5F, 0x02, 0x00];
        private static readonly byte[] MARKER_CONST2 = [0x00, 0x00, 0x00, 0x00];

        static string ReadDeString(BinaryReader reader)
        {
            reader.ReadBytes(2); // == "600A"
            ushort length = reader.ReadUInt16();
            return System.Text.Encoding.UTF8.GetString(reader.ReadBytes(length));
        }
        static string ReadCString(BinaryReader reader)
        {
            List<byte> bytes = new List<byte>();
            while (true)
            {
                byte b = reader.ReadByte();
                if (b == 0)
                {
                    break;
                }
                bytes.Add(b);
            }

            return System.Text.Encoding.ASCII.GetString(bytes.ToArray());
        }


        static float get_save_version(float old_version, uint? new_version)
        {
            if (old_version == -1 && new_version != null)
            {
                if (new_version == 37)
                {
                    return 37.0f;
                }

                return (float)Math.Round(((float)new_version / (1 << 16)), 2);
            }
            return old_version;
        }

        private static void AssertConstant1(BinaryReader stream)
        {
            byte[] marker = stream.ReadBytes(4);
            if (!Enumerable.SequenceEqual(marker, MARKER_CONST1)) { throw new InvalidDataException("Marker constant mismatch"); }
        }

        public static Aoe2ReplayHeaderBasic BasicParse(String replayPath)
        {
            using (var replayStream = File.Open(replayPath, FileMode.Open))
            {
                return BasicParse(replayStream);
            }
        }



        public static Aoe2ReplayHeaderBasic BasicParse(Stream replayStream)
        {

            using (var replayReader = new BinaryReader(replayStream, Encoding.UTF8, false))
            {
                uint header_length = replayReader.ReadUInt32();
                uint chapter_address = replayReader.ReadUInt32();
                if (chapter_address != 0)
                {
                    throw new NotSupportedException("chapter_address not 0");
                }
                byte[] compressed_header = replayReader.ReadBytes((int)header_length - 4 - 4);
                Stream compressed_header_stream = new MemoryStream(compressed_header);
                Stream header_stream = new System.IO.Compression.DeflateStream(compressed_header_stream, System.IO.Compression.CompressionMode.Decompress, false);

                using (var header_reader = new BinaryReader(header_stream, Encoding.ASCII, false))
                {
                    string game_version = ReadCString(header_reader);
                    float old_save_version = header_reader.ReadSingle();
                    uint? new_save_version = (old_save_version == -1) ? header_reader.ReadUInt32() : null;
                    float save_version = get_save_version(old_save_version, new_save_version);

                    uint? build = (save_version >= 25.22f) ? header_reader.ReadUInt32() : null;
                    uint? replay_timestamp = (save_version >= 26.16f) ? header_reader.ReadUInt32() : null;
                    DateTime? replay_datetime = (replay_timestamp != null && replay_timestamp != 0) ? DateTimeOffset.FromUnixTimeSeconds((long)replay_timestamp).DateTime : null;
                    float version = header_reader.ReadSingle();
                    uint interval_version = header_reader.ReadUInt32();
                    uint game_options_version = header_reader.ReadUInt32();
                    uint dlc_count = header_reader.ReadUInt32();
                    List<uint> dlc_ids = new List<uint>();
                    for (int i = 0; i < dlc_count; i++)
                    {
                        dlc_ids.Add(header_reader.ReadUInt32());
                    }
                    uint dataset_ref = header_reader.ReadUInt32();
                    uint difficulty_id = header_reader.ReadUInt32();
                    uint selected_map_id = header_reader.ReadUInt32();
                    uint resolved_map_id = header_reader.ReadUInt32();
                    uint reveal_map = header_reader.ReadUInt32();
                    uint victory_type_id = header_reader.ReadUInt32();
                    uint starting_resources_id = header_reader.ReadUInt32();
                    uint starting_age_id = header_reader.ReadUInt32();
                    uint ending_age_id = header_reader.ReadUInt32();
                    uint game_type_id = header_reader.ReadUInt32();
                    GameType game_type = (GameType)game_type_id;
                    AssertConstant1(header_reader);
                    AssertConstant1(header_reader);
                    float speed = header_reader.ReadSingle();
                    uint treaty_length = header_reader.ReadUInt32();
                    uint population_limit = header_reader.ReadUInt32();
                    uint num_players = header_reader.ReadUInt32();
                    uint unused_player_color = header_reader.ReadUInt32();
                    int victory_amount = header_reader.ReadInt32();
                    byte? unk_byte = (save_version >= 61.5f) ? header_reader.ReadByte() : null;
                    AssertConstant1(header_reader);
                    byte trade_enabled = header_reader.ReadByte();
                    byte team_bonus_disabled = header_reader.ReadByte();
                    byte random_positions = header_reader.ReadByte();
                    byte all_techs = header_reader.ReadByte();
                    byte num_starting_units = header_reader.ReadByte();
                    byte lock_teams = header_reader.ReadByte();
                    byte lock_speed = header_reader.ReadByte();
                    byte multiplayer = header_reader.ReadByte();
                    byte cheats = header_reader.ReadByte();
                    byte record_game = header_reader.ReadByte();
                    byte animals_enabled = header_reader.ReadByte();
                    byte predators_enabled = header_reader.ReadByte();
                    byte turbo_enabled = header_reader.ReadByte();
                    byte shared_exploration = header_reader.ReadByte();
                    byte team_positions = header_reader.ReadByte();
                    uint? sub_game_mode = (save_version >= 13.34f) ? header_reader.ReadUInt32() : null;
                    uint? battle_royale_time = (save_version >= 13.34f) ? header_reader.ReadUInt32() : null;
                    byte? handicap = (save_version >= 25.06f) ? header_reader.ReadByte() : null;
                    byte? unk_byte2 = (save_version >= 50f) ? header_reader.ReadByte() : null;
                    AssertConstant1(header_reader);
                    uint num_player_entries = (save_version >= 37) ? num_players : 8;

                    List<Aoe2ReplayPlayerBasic> players = new List<Aoe2ReplayPlayerBasic>();
                    for (int player = 0; player < num_player_entries; player++)
                    {
                        uint dlc_id = header_reader.ReadUInt32();
                        int color_id = header_reader.ReadInt32();
                        byte selected_color = header_reader.ReadByte();
                        byte selected_team_id = header_reader.ReadByte();
                        byte resolved_team_id = header_reader.ReadByte();
                        byte[] dat_crc = header_reader.ReadBytes(8);
                        byte mp_game_version = header_reader.ReadByte();
                        uint civ_id = header_reader.ReadUInt32();
                        uint? unk_uint3 = (save_version >= 61.5f) ? header_reader.ReadUInt32() : null;
                        string ai_type = ReadDeString(header_reader);
                        byte ai_civ_name_index = header_reader.ReadByte();
                        string ai_name = ReadDeString(header_reader);
                        string name = ReadDeString(header_reader);
                        uint player_type_id = header_reader.ReadUInt32();
                        uint profile_id = header_reader.ReadUInt32();
                        int unk1 = header_reader.ReadInt32();
                        int player_number = header_reader.ReadInt32();
                        uint? hd_rm_elo = (save_version < 25.22f) ? header_reader.ReadUInt32() : null;
                        uint? hd_dm_elo = (save_version < 25.22f) ? header_reader.ReadUInt32() : null;
                        byte prefer_random = header_reader.ReadByte();
                        byte custom_ai = header_reader.ReadByte();
                        byte[]? player_handicap = (save_version >= 25.06f) ? header_reader.ReadBytes(8) : null;
                        players.Add(new Aoe2ReplayPlayerBasic { PlayerNumber = player_number, Name = (name != "") ? name : ai_name, TeamId = resolved_team_id });
                    }

                    return new Aoe2ReplayHeaderBasic { Datetime = replay_datetime, GameType = game_type, Players = players };
                }

            }
        }
    }
}


