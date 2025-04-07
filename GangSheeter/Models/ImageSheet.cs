using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GangSheeter.Models;

public class ImageSheet : ImageModel
{
    public BitmapImage  Source { get; set; }

    public int Copy { get; set; }

    public decimal X { get; set; }

    public decimal Y { get; set; }

    public decimal Rotation { get; set; }
}
