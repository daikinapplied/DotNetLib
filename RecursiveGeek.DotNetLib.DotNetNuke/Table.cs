using System.Web.UI.WebControls;

namespace RecursiveGeek.DotNetLib.DotNetNuke
{
    public static class Table
    {
        public static void RowAddCell(TableRow row, string text, bool wrap = true, string cssClass = "", int columnSpan = 0, int rowSpan = 0)
        {
            var cell = new TableCell
            {
                Text = text,
                Wrap = wrap,
                CssClass = cssClass,
                ColumnSpan = columnSpan,
                RowSpan = rowSpan
            };
            row.Cells.Add(cell);
        }

        public static void RemoveRowsExceptHeader(System.Web.UI.WebControls.Table table)
        {
            for (var index = 1; index < table.Rows.Count; index++)
            {
                table.Rows.RemoveAt(index);
            }
        }
    }
}
