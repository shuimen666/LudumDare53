import csv
import openpyxl
import os

# 设置输出文件夹的路径
output_folder = '../Assets/BundleResources/Tables'

# 如果输出文件夹不存在，则创建它
if not os.path.exists(output_folder):
    os.makedirs(output_folder)


def excel_to_csv(excel_file, csv_file):
    # 读取 Excel 文件
    workbook = openpyxl.load_workbook(excel_file, data_only=True, read_only=True)

    # 遍历所有的工作表
    for sheet_name in workbook.sheetnames:
        worksheet = workbook[sheet_name]

        # 将工作表写入 CSV 文件
        with open(csv_file, 'w', encoding='utf-8', newline='') as f:
            csv_writer = csv.writer(f)

            # 遍历工作表中的所有行和列
            for row in worksheet.rows:
                row_data = []
                for cell in row:
                    row_data.append(cell.value)
                csv_writer.writerow(row_data)
        break

# 遍历当前文件夹中的所有 Excel 文件
for excel_file in os.listdir():
    if excel_file.endswith('.xlsx') and not excel_file.startswith('~'):
        print(excel_file)
        # 计算输出文件的路径
        csv_file = os.path.join(output_folder, excel_file.replace('.xlsx', '.csv'))

        # 将 Excel 文件转换为 CSV 格式
        excel_to_csv(excel_file, csv_file)

print("Finished.")