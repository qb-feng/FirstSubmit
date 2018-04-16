using System.IO;
using Tangzx.ABSystem;
using System.Collections.Generic;

public class AssetBundleDataWriter
{
    public void Save(string path, List<AssetBundleData> data)
    {
		using(var fs = new FileStream(path, FileMode.Create))
		{
			Save(fs, data);
			fs.Close ();
		}
    }

    public virtual void Save(Stream stream, List<AssetBundleData> data)
    {
        StreamWriter sw = new StreamWriter(stream);
        //写入文件头判断文件类型用，ABDT 意思即 Asset-Bundle-Data-Text
        sw.WriteLine("ABDT");

        for (int i = 0; i < data.Count; i++)
        {
            AssetBundleData ab = data[i];
            //debug name
            sw.WriteLine(ab.debugName);
            //bundle name
            sw.WriteLine(ab.fullName);
            //File Name
            sw.WriteLine(ab.shortName);
            //hash
            sw.WriteLine(ab.hash);
            //type
            sw.WriteLine((int)ab.compositeType);
            //写入依赖信息
            sw.WriteLine(ab.dependencies.Length);

            foreach (string dep in ab.dependencies)
            {
                sw.WriteLine(dep);
            }
            sw.WriteLine("<------------->");
        }
        sw.Close();
    }
}
