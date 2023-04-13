using Aspose.Cells;
using Camera.models;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;

namespace Camera.services
{
    public class TemplateService
    {
        private static  int FIRST_SENSOR_ROW = 19;


        private Workbook workbook { get; set; } 
        private Worksheet worksheet { get; set; }
        public TemplateService() {
            workbook = new Workbook();
            worksheet = workbook.Worksheets[0];
        }
        public void WriteToTemplate(string operatorName, string roomName, List<GasSensorReading> gasSensorReadings, string outsideSnapshot, string insideSnaphot)
        {
            //FileInputStream templateFile = new FileInputStream(new File("so_template.xlsx"));
            string folder = Environment.CurrentDirectory + @"\user\reports\";
            if (!System.IO.Directory.Exists(folder))
            {
                System.IO.Directory.CreateDirectory(folder);
            }


            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage excel = new ExcelPackage();
            var worksheet = excel.Workbook.Worksheets.Add("Report");


            SetMetaDataCells(worksheet, operatorName, roomName);
            SetGasReadingCells(worksheet, gasSensorReadings);
            SetImages(worksheet, outsideSnapshot, insideSnaphot);


            MemoryStream stream = new MemoryStream();
            excel.SaveAs(stream);

            string date = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
            string reportFilename = "Safety_report_" + date + ".xlsx";
            string outputFile = System.IO.Path.Combine(folder, reportFilename);

            File.WriteAllBytes(outputFile, excel.GetAsByteArray());






            Process.Start(outputFile);
        }
        private void SetMetaDataCells(ExcelWorksheet worksheet, string operatorName, string roomName)
        {
            DateTime currentTime = DateTime.UtcNow;
            worksheet.Cells[8, 3].Value = (currentTime.ToShortDateString());
            worksheet.Cells[10, 3].Value = (currentTime.ToShortTimeString());
            worksheet.Cells[12, 3].Value = (operatorName);
            worksheet.Cells[14, 3].Value = (roomName);
        }

        private void SetImages(ExcelWorksheet worksheet, string outsideSnapshot, string insideSnaphot)
        {

            if(File.Exists(outsideSnapshot))
            {
                ExcelPicture pic = worksheet.Drawings.AddPicture("Outside", outsideSnapshot);   
                pic.SetPosition(1, 2, 7, 2);
            }

            if(File.Exists(insideSnaphot))
            {
                ExcelPicture pic = worksheet.Drawings.AddPicture("Inside", insideSnaphot);
                pic.SetPosition(20, 2, 7, 2);
            }
        }

   

        private void SetGasReadingCells(ExcelWorksheet worksheet, List<GasSensorReading> gasSensorReadings)
        {
            
            // sensor reading cells being filled in
            int sensorCounter = 0;
            foreach (GasSensorReading gasSensorReading in gasSensorReadings)
            {
                worksheet.Cells[FIRST_SENSOR_ROW + sensorCounter, 1].Value = (gasSensorReading.type);
                worksheet.Cells[FIRST_SENSOR_ROW + sensorCounter, 2].Value = (gasSensorReading.value);

                sensorCounter++;
            }
        }

        
    }
}
