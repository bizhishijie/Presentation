using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net;
using System.Web;

namespace Presentation
{
    public partial class 查询交通情况 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String path = @"C:\Users\sharuicheng\source\repos\Presentation\Presentation\map\json\province";
            var files = Directory.GetFiles(path, "*.json");
            List<List<string>> CityName = new List<List<string>>(); ;
            List<List<List<double>>> CityPlace = new List<List<List<double>>>();
            foreach (var file in files)
            {
                CityName.Add(LoadJsonName(file));
                CityPlace.Add(LoadJsonPlace(file));
            }
            if (!IsPostBack)
            {
                File.Delete(@"C:\Users\sharuicheng\source\repos\Presentation\Presentation\data\temp.jpg");
                DataTable dt = new DataTable();
                DataSet ds = new DataSet();
                dt.Columns.Add("cityName", typeof(string));
                dt.Columns.Add("value", typeof(string));
                for (int i =0;i<CityName.Count;i++)
                {
                    for (int j = 0; j < CityName[i].Count; j++)
                    {
                        DataRow dr = dt.NewRow();
                        dr["cityName"] = CityName[i][j];
                        dr["value"] = string.Format("{0},{1}",i,j);//城市编号
                        dt.Rows.Add(dr);
                    }
                }
                    if (dt.Rows.Count > 0)
                    {
                        DropDownList1.DataTextField = "cityName";
                        DropDownList1.DataValueField = "value";
                        DropDownList1.DataSource = dt;
                        DropDownList1.DataBind();
                        DropDownList1.SelectedIndex = dt.Rows.Count - 1;
                    }
            }
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            String path = @"C:\Users\sharuicheng\source\repos\Presentation\Presentation\map\json\province";
            var files = Directory.GetFiles(path, "*.json");
            List<List<string>> CityName = new List<List<string>>(); ;
            List<List<List<double>>> CityPlace = new List<List<List<double>>>();
            foreach (var file in files)
            {
                CityName.Add(LoadJsonName(file));
                CityPlace.Add(LoadJsonPlace(file));
            }
            string cityKey = DropDownList1.SelectedValue;
            string[] index = cityKey.Split(',');
            int i = int.Parse(index[0]);
            int j = int.Parse(index[1]);
            string url = string.Format(@"https://restapi.amap.com/v3/staticmap?location={0},{1}&zoom=10&traffic=1&size=500*500&markers=mid,,A:116.481485,39.990464&scale=1&key=fca63286362d9fa8f7bb88574f704da1",
                CityPlace[i][j][0], CityPlace[i][j][1]);
            HttpDownloadFile(url, @"C:\Users\sharuicheng\source\repos\Presentation\Presentation\data\temp.jpg");
            Image1.ImageUrl = @"C:\Users\sharuicheng\source\repos\Presentation\Presentation\data\temp.jpg";
            Response.Write(GetRootPath());
        }
        protected List<string> LoadJsonName(string filePath)
        {
            using (StreamReader r = new StreamReader(filePath))
            {
                List<string> cityName = new List<string>();
                string json = r.ReadToEnd();
                JObject jo = (JObject)JsonConvert.DeserializeObject(json);
                string name = string.Empty;
                try
                {
                    for (int i = 0; i < jo["features"].Count(); i++)
                    {
                        name = Convert.ToString(jo["features"][i]["properties"]["name"]);
                        cityName.Add(name);
                    }
                }
                finally { }
                return cityName;
            }
        }
        protected List<List<double>> LoadJsonPlace(string filePath)
        {
            using (StreamReader r = new StreamReader(filePath))
            {
                List<List<double>> place = new List<List<double>>(); ;
                string json = r.ReadToEnd();
                JObject jo = (JObject)JsonConvert.DeserializeObject(json);
                string name = string.Empty;
                for (int i = 0; i < jo["features"].Count(); i++)
                {
                    try
                    {
                        List<double> temp = new List<double>();
                        temp.Add(Convert.ToDouble(jo["features"][i]["properties"]["cp"][0]));
                        temp.Add(Convert.ToDouble(jo["features"][i]["properties"]["cp"][1]));
                        place.Add(temp);
                    }
                    catch(Exception )
                    {
                    }
                }
                return place;
            }
        }
        public string GetRootPath()
        {
            // 是否为SSL认证站点
            string secure = HttpContext.Current.Request.ServerVariables["HTTPS"];
            string httpProtocol = (secure == "on" ? "https://" : "http://");
            // 服务器名称
            string serverName = HttpContext.Current.Request.ServerVariables["Server_Name"];
            string port = HttpContext.Current.Request.ServerVariables["SERVER_PORT"];
            // 应用服务名称
            string applicationName = HttpContext.Current.Request.ApplicationPath;
            return httpProtocol + serverName + (port.Length > 0 ? ":" + port : string.Empty) + applicationName;
        }
        protected static string HttpDownloadFile(string url, string path)
        {
            // 设置参数
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            Stream responseStream = response.GetResponseStream();
            //创建本地文件写入流
            Stream stream = new FileStream(path, FileMode.Create);
            byte[] bArr = new byte[1024];
            int size = responseStream.Read(bArr, 0, (int)bArr.Length);
            while (size > 0)
            {
                stream.Write(bArr, 0, size);
                size = responseStream.Read(bArr, 0, (int)bArr.Length);
            }
            stream.Close();
            responseStream.Close();
            return path;
        }
    }
}