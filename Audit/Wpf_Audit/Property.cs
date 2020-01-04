using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf_Audit
{
    class Property
    {
        public static string GetRank(int num)
        {
            string rank;
            switch (num)
            {
                case 10: rank = "等待审核"; break;
                case 1: rank = "一级审核通过"; break;
                case 2: rank = "二级审核通过"; break;
                case 3: rank = "三级审核通过"; break;
                case 4: rank = "四级审核通过"; break;
                default: rank = string.Empty; break;
            }
            return rank;
        }

        public static string GetStatus(int num)
        {
            string status;
            switch (num)
            {
                case 10: status = "等待审核"; break;
                case 0: status = "该记录已删除"; break;
                case 1: status = "一级审核通过"; break;
                case 2: status = "二级审核通过"; break;
                case 3: status = "三级审核通过"; break;
                case 4: status = "四级审核通过"; break;
                case 5: status = "一级审核未通过"; break;
                case 6: status = "二级审核未通过"; break;
                case 7: status = "三级审核未通过"; break;
                case 8: status = "四级审核未通过"; break;
                case 9: status = "申请已提交，等待上传文件"; break;
                case 11: status = "已还款"; break;
                default: status = string.Empty; break;
            }
            return status;
        }

        public static string GetIsPassed(int num)
        {
            string isPassed;
            switch (num)
            {
                case 0: isPassed = "否"; break;
                case 1: isPassed = "是"; break;
                default: isPassed = string.Empty; break;
            }
            return isPassed;
        }

        public static string GetIsInGov(int num)
        {
            string isInGov;
            switch (num)
            {
                case 0: isInGov = "否"; break;
                case 1: isInGov = "是"; break;
                default: isInGov = string.Empty; break;
            }
            return isInGov;
        }

    }
}
