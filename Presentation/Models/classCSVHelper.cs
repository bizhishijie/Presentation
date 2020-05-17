using System;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.IO;
using System.Text;

namespace CSV
{
    public class classCSVHelper
    {
        #region Fields
        string _fileName;
        DataTable _dataSource;//数据源
        string[] _titles = null;//列标题
        string[] _fields = null;//字段名

        #endregion

        #region .ctor
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dataSource">数据源</param>
        public classCSVHelper()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="titles">要输出到 Excel 的列标题的数组</param>
        /// <param name="fields">要输出到 Excel 的字段名称数组</param>
        /// <param name="dataSource">数据源</param>
        public classCSVHelper(string[] titles, string[] fields, DataTable dataSource)
            : this(titles, dataSource)
        {
            if (fields == null || fields.Length == 0)
                throw new ArgumentNullException("fields");
            if (titles.Length != fields.Length)
                throw new ArgumentException("titles.Length != fields.Length", "fields");

            _fields = fields;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="titles">要输出到 Excel 的列标题的数组</param>
        /// <param name="dataSource">数据源</param>
        public classCSVHelper(string[] titles, DataTable dataSource)
            : this(dataSource)
        {
            if (titles == null || titles.Length == 0)
                throw new ArgumentNullException("titles");

            _titles = titles;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dataSource">数据源</param>
        public classCSVHelper(DataTable dataSource)
        {
            if (dataSource == null)
                throw new ArgumentNullException("dataSource");
            // maybe more checks needed here (IEnumerable, IList, IListSource, ) ???
            // 很难判断，先简单的使用 DataTable

            _dataSource = dataSource;
        }

        #endregion



        #region  导出到CSV文件并且提示下载


        /// <summary>
        /// 获取CSV导入的数据
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="fileName">文件名称</param>
        /// <returns></returns>
        public DataTable GetCsvData(string filePath, string fileName)
        {
            if (!fileName.Contains(".csv"))
                fileName = fileName + ".csv";
            string path = Path.Combine(filePath, fileName);
            string connString = @"Driver={Microsoft Text Driver (*.txt; *.csv)};Dbq=" + filePath + ";Extensions=asc,csv,tab,txt;";
            try
            {
                using (OdbcConnection odbcConn = new OdbcConnection(connString))
                {
                    odbcConn.Open();
                    OdbcCommand oleComm = new OdbcCommand();
                    oleComm.Connection = odbcConn;
                    oleComm.CommandText = "select * from [" + fileName + "#csv]";
                    OdbcDataAdapter adapter = new OdbcDataAdapter(oleComm);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds, fileName);
                    return ds.Tables[0];
                    odbcConn.Close();
                }
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                throw ex;
            }
        }
        #endregion

        #region 返回写入CSV的字符串
        /// <summary>
        /// 返回写入CSV的字符串
        /// </summary>
        /// <returns></returns>
        private string ExportCSV()
        {
            if (_dataSource == null)
                throw new ArgumentNullException("dataSource");

            StringBuilder strbData = new StringBuilder();
            if (_titles == null)
            {
                //添加列名
                foreach (DataColumn column in _dataSource.Columns)
                {
                    strbData.Append(column.ColumnName + ",");
                }
                strbData.Append("\n");
                foreach (DataRow dr in _dataSource.Rows)
                {
                    for (int i = 0; i < _dataSource.Columns.Count; i++)
                    {
                        strbData.Append(dr[i].ToString() + ",");
                    }
                    strbData.Append("\n");
                }
                return strbData.ToString();
            }
            else
            {
                foreach (string columnName in _titles)
                {
                    strbData.Append(columnName + ",");
                }
                strbData.Append("\n");
                if (_fields == null)
                {
                    foreach (DataRow dr in _dataSource.Rows)
                    {
                        for (int i = 0; i < _dataSource.Columns.Count; i++)
                        {
                            strbData.Append(dr[i].ToString() + ",");
                        }
                        strbData.Append("\n");
                    }
                    return strbData.ToString();
                }
                else
                {
                    foreach (DataRow dr in _dataSource.Rows)
                    {
                        for (int i = 0; i < _fields.Length; i++)
                        {
                            strbData.Append(_fields[i].ToString() + ",");
                        }
                        strbData.Append("\n");
                    }
                    return strbData.ToString();
                }
            }
        }
        #endregion

        #region 得到一个随意的文件名
        /// <summary>
        /// 得到一个随意的文件名
        /// </summary>
        /// <returns></returns>
        private string GetRandomFileName()
        {
            Random rnd = new Random((int)(DateTime.Now.Ticks));
            string s = rnd.Next(Int32.MaxValue).ToString();
            return DateTime.Now.ToShortDateString() + "_" + s + ".csv";
        }
        #endregion

         /// <summary>
        /// 读取CSV文件通过文本格式
        /// </summary>
        /// <param name="strpath"></param>
        /// <returns></returns>
        public DataTable readCsvTxt(string strpath,Encoding en)
        {
            int intColCount = 0;
            bool blnFlag = true;
            DataTable mydt = new DataTable("myTableName");

            DataColumn mydc;
            DataRow mydr;

            string strline;
            string [] aryline;

            System.IO.StreamReader mysr = new System.IO.StreamReader(strpath, en);

            while((strline = mysr.ReadLine()) != null)
            {
            aryline = strline.Split(',');

            if (blnFlag)
            {
            blnFlag = false;
            intColCount = aryline.Length;
            for (int i = 0; i < aryline.Length; i++ )
            {
            mydc = new DataColumn(aryline[i]);
            mydt.Columns.Add(mydc);
            }
            }

            mydr = mydt.NewRow();
            for (int i = 0; i < intColCount; i++ )
            {
            mydr[i] = aryline[i];
            }
            mydt.Rows.Add(mydr);
            }

            return mydt;
        }

        /// <summary>
        /// 利用SQL查询CSV
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public DataTable readCsvSql(string filepath)
        {
            FileInfo fi = new FileInfo(filepath);
            string path = fi.Directory.FullName;
            string filename = fi.Name;
            string strconn = string.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=Text;", path);
            string sql = string.Format("SELECT * FROM [{0}]", filename);
            using (OleDbConnection conn = new OleDbConnection(strconn))
            {
                DataTable dtTable = new DataTable();
                OleDbDataAdapter adapter = new OleDbDataAdapter(sql, conn);
                try
                {
                    adapter.Fill(dtTable);
                   
                }
                catch (Exception ex)
                {
                    dtTable = new DataTable();
                    throw ex;
                }
                return dtTable;
            }

        }

        /// <summary>
        /// 读取CSV文件
        /// </summary>
        /// <param name="mycsvdt"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public bool readCsvFile(ref DataTable mycsvdt, string filepath)
        {
            string strpath = filepath; //csv文件的路径
            try
            {
                int intColCount = 0;
                bool blnFlag = true;

                DataColumn mydc;
                DataRow mydr;

                string strline;
                string[] aryline;
                StreamReader mysr = new StreamReader(strpath, System.Text.Encoding.Default);

                while ((strline = mysr.ReadLine()) != null)
                {
                    aryline = strline.Split(new char[] { ',' });

                    //给datatable加上列名
                    if (blnFlag)
                    {
                        blnFlag = false;
                        intColCount = aryline.Length;
                        int col = 0;
                        for (int i = 0; i < aryline.Length; i++)
                        {
                            col = i + 1;
                            mydc = new DataColumn(col.ToString());
                            mycsvdt.Columns.Add(mydc);
                        }
                    }

                    //填充数据并加入到datatable中
                    mydr = mycsvdt.NewRow();
                    for (int i = 0; i < intColCount; i++)
                    {
                        mydr[i] = aryline[i];
                    }
                    mycsvdt.Rows.Add(mydr);
                }
                return true;

            }
            catch (Exception e)
            {
                //throw (Stack.GetErrorStack(strpath + "读取CSV文件中的数据出错." + e.Message, "OpenCSVFile("));
                return false;
            }
        }
    }
}
