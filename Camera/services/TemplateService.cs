using Aspose.Cells;
using Camera.models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            SetMetaDataCells(operatorName, roomName);
            SetGasReadingCells(gasSensorReadings);
            SetImages(outsideSnapshot, insideSnaphot);

            string date = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
            string reportFilename = "Safety_report_" + date + ".xlsx";
            string outputFile = System.IO.Path.Combine(folder, reportFilename);

            workbook.Save(outputFile);

            


            

            Process.Start(outputFile);
        }

        private void SetImages(string outsideSnapshot, string insideSnaphot)
        {
            if(!File.Exists(outsideSnapshot) || !File.Exists(insideSnaphot)) {
                return;
            }
            worksheet.Pictures.Add(1, 7, 28, 41, outsideSnapshot);
            worksheet.Pictures.Add(9, 15, 28, 41, insideSnaphot);
        }

   

        private void SetGasReadingCells(List<GasSensorReading> gasSensorReadings)
        {
            Style sensorTypeStyle = workbook.CreateStyle();
            Font font = workbook.GetFonts().FirstOrDefault();
            font.IsBold = true; // if no workies try setBoldWeight(Font.BOLDWEIGHT_BOLD)
            sensorTypeStyle.HorizontalAlignment = TextAlignmentType.Center;
            // sensor reading cells being filled in
            int sensorCounter = 0;
            foreach (GasSensorReading gasSensorReading in gasSensorReadings)
            {

                Cell sensorTypeCell = worksheet.Cells[FIRST_SENSOR_ROW + sensorCounter, 1];
                Cell sensorValueCell = worksheet.Cells[FIRST_SENSOR_ROW + sensorCounter, 2];

                sensorTypeCell.SetStyle(sensorTypeStyle);
                sensorTypeCell.PutValue(gasSensorReading.type);
                sensorValueCell.PutValue(gasSensorReading.value);

                sensorCounter++;
            }
        }

        private void SetMetaDataCells(string operatorName, string roomName)
        {
            Cell dateCell = worksheet.Cells[8, 3];
            Cell timeCell = worksheet.Cells[10, 3];
            Cell operatorCell = worksheet.Cells[12, 3];
            Cell locationCell = worksheet.Cells[14, 3];
            DateTime currentTime = DateTime.UtcNow;
            dateCell.PutValue(currentTime.ToShortDateString());
            timeCell.PutValue(currentTime.ToShortTimeString());
            operatorCell.PutValue(operatorName);
            locationCell.PutValue(roomName);
        }
    }
}
