using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Numerics;
using System.Text;
using System.Globalization;
using System.Runtime.CompilerServices;
using Assets._ExtendedPrinter.Scripts.Helper;

namespace Assets._ExtendedPrinter.Scripts.SlicingService
{
    [Serializable]
    public class PrusaSlicerCLICommands
    {
        public static PrusaSlicerCLICommands Default { get { return new PrusaSlicerCLICommands() { ExportGCode = true, SupportMaterial = false, LayerHeight = 0.2f, FillDensity = 0.5f, GcodeComments = true, Loglevel = 3 }; } }

        #region Properties
        [CLICommand("--export-gcode")]
        public bool? ExportGCode;

        [CLICommand("--export-obj")]
        public bool? ExportOBJ;

        [CLICommand("-s")]
        public bool? Slice;

        [CLICommand("--single-isntance")]
        public bool? SingleInstance;

        [CLICommand("--repair")]
        public bool? Repair;

        [CLICommand("--support-material")]
        public bool? SupportMaterial;

        [CLICommand("--rotate")]
        public float? Rotate;

        [CLICommand("--rotate-x")]
        public float? RotateX;

        [CLICommand("--rotate-y")]
        public float? RotateY;

        [CLICommand("--scale")]
        public float? Scale;

        [CLICommand("--layer-height")]
        public float? LayerHeight;

        [CLICommand("--load")]
        public string LoadConfigFile;

        [CLICommand("-o")]
        public string Output;

        [CLICommand("--save")]
        public string SaveConfigFile;

        [CLICommand("--loglevel")]
        public int? Loglevel;

        [CLICommand("--fill-density")]
        public float? FillDensity;


        [CLICommand("--scale-to-fit")]
        public SerializableVector3 ScaleToFit;

        [CLICommand("--align-xy")]
        public SerializableVector2 AlignXY;

        [CLICommand("--center")]
        public SerializableVector2 Center;

        [CLICommand("--gcode-comments")]
        public bool? GcodeComments;

        [CLICommand("")]
        public string File;
        #endregion

        public bool isValid()
        {

            if (String.IsNullOrEmpty(File))
                return false;

#if !DEBUG
            if (!FillDensity.HasValue)
                return false;
            
            if (FillDensity < 0f || FillDensity > 1f)
                return false;

            if (!LayerHeight.HasValue)
                return false;

            if (LayerHeight < 0.05f || LayerHeight > 0.3f)
                return false;

            if (AlignXY!=null && (AlignXY.X < 0f || AlignXY.Y < 0f))
                return false;
            if (Center!=null &&(Center.X < 0f || Center.Y < 0f))
                return false;
#endif
            return true;
        }

        public override string ToString()
        {
            StringBuilder commandBuilder = new StringBuilder();

            //get all properties
            var props = typeof(PrusaSlicerCLICommands).GetFields();

            foreach (var prop in props)
            {
                if (prop.GetValue(this) == null) continue;

                var attributes = prop.GetCustomAttributes(false);

                if (attributes.Length <= 0) continue;

                var CLICommand = attributes.First() as CLICommand;

                //there could be the rare case that GetProperties gets an property that has no CLICommand attribute -> ignore all of these properties
                if (CLICommand == null) continue;

                if (prop.FieldType == typeof(bool?))
                {
                    // Add command only if value is true
                    if ((bool?)prop.GetValue(this) == true)
                    {
                        commandBuilder.Append(CLICommand.GetCommand());
                        commandBuilder.Append(" ");
                    }
                }
                else
                {
                    //first add command and add value later
                    commandBuilder.Append(CLICommand.GetCommand());
                    commandBuilder.Append(" ");

                    if (prop.FieldType == typeof(float?))
                    {
                        commandBuilder.Append(((float)prop.GetValue(this)).ToString("F", CultureInfo.InvariantCulture));
                        commandBuilder.Append(" ");
                    }
                    else if (prop.FieldType == typeof(string))
                    {
                        commandBuilder.Append((string)prop.GetValue(this));
                        commandBuilder.Append(" ");
                    }
                    else if (prop.FieldType == typeof(int?))
                    {
                        commandBuilder.Append(((int)prop.GetValue(this)).ToString());
                        commandBuilder.Append(" ");
                    }
                    else if (prop.FieldType == typeof(SerializableVector2))
                    {
                        float x, y;
                        var tmp = (SerializableVector2)prop.GetValue(this);
                        x = tmp.X;
                        y = tmp.Y;
                        commandBuilder.Append(x.ToString("F", CultureInfo.InvariantCulture));
                        commandBuilder.Append(",");
                        commandBuilder.Append(y.ToString("F", CultureInfo.InvariantCulture));
                        commandBuilder.Append(" ");

                    }
                    else if (prop.FieldType == typeof(SerializableVector3))
                    {
                        float x, y, z;
                        var tmp = (SerializableVector3)prop.GetValue(this);
                        x = tmp.X;
                        y = tmp.Y;
                        z = tmp.Z;
                        commandBuilder.Append(x.ToString("F", CultureInfo.InvariantCulture));
                        commandBuilder.Append(",");
                        commandBuilder.Append(y.ToString("F", CultureInfo.InvariantCulture));
                        commandBuilder.Append(",");
                        commandBuilder.Append(z.ToString("F", CultureInfo.InvariantCulture));
                        commandBuilder.Append(" ");
                    }
                }
            }

            return commandBuilder.ToString();
        }
    }


    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Property)]
    public class CLICommand : System.Attribute
    {
        string command;

        public CLICommand(string command)
        {
            this.command = command;
        }

        public string GetCommand()
        {
            return command;
        }
    }
}

