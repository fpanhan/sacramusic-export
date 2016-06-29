using System;
using System.Data.SqlClient;
using System.Text;

namespace exportwpcontent
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var conn = new SqlConnection(@"server = (local)\SQLEXPRESS; database = evonext; Trusted_Connection = True");
            var sqlQuery = "SELECT c.IdCan, n.DtiNot, n.HoiNot, n.TitNot, n.AutNot, n.NotNot,  * ";
            sqlQuery += "FROM dbo.NTO n ";
            sqlQuery += "INNER JOIN dbo.CAN c ON c.IdCan = n.IdCan ";
            sqlQuery += "INNER JOIN dbo.SEC s ON s.IdSec = n.IdSec ";
            sqlQuery += "INNER JOIN dbo.CAT c2 ON c2.IdCat = n.IdCat ";
            //sqlQuery += "WHERE n.IdCan IN (3, 4, 5, 6, 7, 8, 9) ";
            sqlQuery += "WHERE n.IdCan IN (2) ";
            sqlQuery += "ORDER BY n.DtiNot";

            using (var command = new SqlCommand(sqlQuery, conn))
            {
                conn.Open();
                var dr = command.ExecuteReader();
                var dtSchema = dr.GetSchemaTable();
                var strRow = new StringBuilder();
                var finisher = Environment.NewLine;
                //finisher = string.Empty;

                //strRow.AppendLine("<?xml version='1.0' encoding='ISO-8859-1'?>");
                //strRow.AppendLine("<rss version='2.0'>");
                //strRow.AppendLine("<channel>");
                //strRow.AppendLine("<title>Sacramusic &#8211; Música católica, Espiritualidade</title>");
                //strRow.AppendLine("<description>Sacramusic &#8211; Música católica, Espiritualidade</description>");
                //strRow.AppendLine("<link>http://www.sacramusic.com</link>");
                //strRow.AppendLine("<language>pt-br</language>");
                //strRow.AppendLine("<copyright>Copyright (C) 1999-2016 sacramusic.com</copyright>");

                strRow.AppendLine("pubDate;category;title;description");

                while (dr.Read())
                {
                    var channel = string.Empty;

                    switch (dr["IdCan"].ToString())
                    {
                        case "2":
                            {
                                channel = "Atualidade";
                                break;
                            }
                        case "3":
                        case "9":
                            {
                                channel = "MÚSICA CATÓLICA";
                                break;
                            }
                        case "4":
                        case "5":
                            {
                                channel = "DEUS";
                                break;
                            }
                        case "6":
                        case "7":
                            {
                                channel = "CULTURA";
                                break;
                            }
                        case "8":
                            {
                                channel = "MÍDIA";
                                break;
                            }
                        default:
                            {
                                channel = string.Empty;
                                break;
                            }
                    }

                    var hour = dr["HoiNot"].ToString().Replace("h", ":");
                    if (hour.Length < 8) hour += ":00";
                    var dateHour = (((DateTime)(dr["DtiNot"])).Date).ToString("u").Substring(0, 11) + hour;
                    var tittle = dr["TitNot"];
                    var news = dr["NotNot"].ToString()
                                    .Replace(Environment.NewLine, string.Empty)
                                    .Replace("--", "-")
                                    .Replace(".-", " -")
                                    .Replace("“", "\"")
                                    .Replace("”", "\"")
                                    .Replace("'", "")
                                    .Replace(";", ". ")
                                    .Replace(@"\", "")
                                    .Replace(@"//", "")
                                    .Replace(@"/*", "")
                                    .Replace("<i>", string.Empty)
                                    .Replace("</i>", string.Empty)
                                    .Replace("<b>", string.Empty)
                                    .Replace("</b>", string.Empty)
                                    .Replace("<p>", "<br><br>")
                                    .Replace("</p>", string.Empty)
                                    .Replace("<br />", "<br>")
                                    .Replace("<br/>", "<br>")
                                    ;

                    strRow.AppendFormat("{0};{1};{2};{3}{4}", dateHour, channel, tittle, news, finisher);

                    //strRow.AppendLine("<item>");
                    //strRow.AppendFormat("<pubDate>{0}</pubDate>{1}", dateHour, finisher);
                    //strRow.AppendFormat("<category>{0}</category>{1}", channel, finisher);
                    //strRow.AppendFormat("<title>{0}</title>{1}", tittle, finisher);
                    //strRow.AppendFormat("<description>{0}</description>{1}", news, finisher);
                    ////strRow.AppendFormat("<title><![CDATA[{0}]]></title>{1}", tittle, finisher);
                    ////strRow.AppendFormat("<description><![CDATA[{0}]]></description>{1}", news, finisher);
                    //strRow.AppendLine("</item>");
                }

                //strRow.AppendLine("</channel>");
                //strRow.AppendLine("</rss>");

                var tst = strRow.ToString();
                conn.Close();
            }

            Console.ReadKey();
        }

        public static string DateString(DateTime pubDate)
        {
            return pubDate.ToString("ddd',' d MMM yyyy HH':'mm':'ss") + " " + pubDate.ToString("zzzz").Replace(":", "");
        }
    }
}
