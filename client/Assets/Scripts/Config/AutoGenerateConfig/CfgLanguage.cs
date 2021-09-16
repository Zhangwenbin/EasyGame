//--------------------------------------------------
// 自动生成  请勿修改
// 研发人员实现LANG多语言接口
//--------------------------------------------------
using MotionFramework.IO;
using System.Collections.Generic;
using MotionFramework.Config;

[Config(nameof(EConfigType.Language))]
public class CfgLanguageTable : ConfigTable
{
	public string Lang { protected set; get; }
	public string Comment1 { protected set; get; }

	public override void ReadByte(ByteBuffer byteBuf)
	{
		Id = byteBuf.ReadUTF().GetHashCode();
		Lang = byteBuf.ReadUTF();
		Comment1 = byteBuf.ReadUTF();
	}
}

[Config(nameof(EConfigType.Language))]
public partial class CfgLanguage : AssetConfig
{
	protected override ConfigTable ReadTable(ByteBuffer byteBuffer)
	{
		CfgLanguageTable table = new CfgLanguageTable();
		table.ReadByte(byteBuffer);
		return table;
	}
}
