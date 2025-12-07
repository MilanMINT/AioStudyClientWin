using AioStudy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.Core.Util.Grades
{
    public static class GradeHelper
    {
        public static class Math
        {
            public static double CalculateGPA(IEnumerable<Module> modules, bool includeFailedExams)
            {
                double gpa = 0.0;
                int creditsSum = 0;

                foreach (var module in modules)
                {
                    if (module.Grade.HasValue && module.ModuleCredits.HasValue)
                    {
                        if (module.IsKeyCompetence)
                        {
                            continue;
                        }

                        if (!includeFailedExams && module.Grade.Value > 4.0f)
                        {
                            continue;
                        }

                        gpa += module.Grade.Value * module.ModuleCredits.Value;
                        creditsSum += module.ModuleCredits.Value;
                    }
                }
                return creditsSum > 0 ? gpa / creditsSum : 0.0;
            }

            public static int CalculateTotalCredits(IEnumerable<Module> modules, bool includeFailedExams)
            {
                int totalCredits = 0;
                foreach (var module in modules)
                {
                    if (module.IsKeyCompetence && module.ModuleCredits.HasValue)
                    {
                        totalCredits += module.ModuleCredits.Value;
                        continue;
                    }

                    if (module.Grade.HasValue && module.Grade.Value > 0 && module.ModuleCredits.HasValue)
                    {
                        if (!includeFailedExams && module.Grade.Value > 4.0f)
                        {
                            continue;
                        }
                        totalCredits += module.ModuleCredits.Value;
                    }
                }
                return totalCredits;
            }
        }
    }
}