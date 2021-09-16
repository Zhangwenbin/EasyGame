//--------------------------------------------------
// 自动生成  请勿修改
// 研发人员实现LANG多语言接口
//--------------------------------------------------
using MotionFramework.IO;
using System.Collections.Generic;
using MotionFramework.Config;

[Config(nameof(EConfigType.Level))]
public class CfgLevelTable : ConfigTable
{
	public int ChapterId { protected set; get; }
	public int LevelSerial { protected set; get; }
	public string LevelTitle { protected set; get; }
	public string LevelRes { protected set; get; }
	public List<int> SceneIds { protected set; get; }
	public VectorI3 BagGridDim { protected set; get; }
	public int ChanceTaken { protected set; get; }
	public List<int> Target1 { protected set; get; }
	public string TargetName1 { protected set; get; }
	public int TargetNum1 { protected set; get; }
	public List<int> Target2 { protected set; get; }
	public string TargetName2 { protected set; get; }
	public int TargetNum2 { protected set; get; }
	public int GoodLine { protected set; get; }
	public int PerfectLine { protected set; get; }

	public override void ReadByte(ByteBuffer byteBuf)
	{
		Id = byteBuf.ReadInt();
		ChapterId = byteBuf.ReadInt();
		LevelSerial = byteBuf.ReadInt();
		LevelTitle = byteBuf.ReadUTF();
		LevelRes = byteBuf.ReadUTF();
		SceneIds = byteBuf.ReadListInt();
		BagGridDim = VectorI3.Parse(byteBuf);
		ChanceTaken = byteBuf.ReadInt();
		Target1 = byteBuf.ReadListInt();
		TargetName1 = byteBuf.ReadUTF();
		TargetNum1 = byteBuf.ReadInt();
		Target2 = byteBuf.ReadListInt();
		TargetName2 = byteBuf.ReadUTF();
		TargetNum2 = byteBuf.ReadInt();
		GoodLine = byteBuf.ReadInt();
		PerfectLine = byteBuf.ReadInt();
	}
}

[Config(nameof(EConfigType.Level))]
public partial class CfgLevel : AssetConfig
{
	protected override ConfigTable ReadTable(ByteBuffer byteBuffer)
	{
		CfgLevelTable table = new CfgLevelTable();
		table.ReadByte(byteBuffer);
		return table;
	}
}
