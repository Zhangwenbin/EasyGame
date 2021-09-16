//--------------------------------------------------
// 自动生成  请勿修改
// 研发人员实现LANG多语言接口
//--------------------------------------------------
using MotionFramework.IO;
using System.Collections.Generic;
using MotionFramework.Config;

[Config(nameof(EConfigType.AutoGenerateLanguage))]
public class CfgAutoGenerateLanguageTable : ConfigTable
{
	public string Lang { protected set; get; }

	public override void ReadByte(ByteBuffer byteBuf)
	{
		Id = byteBuf.ReadInt();
		Lang = byteBuf.ReadUTF();
	}
}

[Config(nameof(EConfigType.AutoGenerateLanguage))]
public partial class CfgAutoGenerateLanguage : AssetConfig
{
	protected override ConfigTable ReadTable(ByteBuffer byteBuffer)
	{
		CfgAutoGenerateLanguageTable table = new CfgAutoGenerateLanguageTable();
		table.ReadByte(byteBuffer);
		return table;
	}
}
