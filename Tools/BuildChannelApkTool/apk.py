# coding=utf-8
import zipfile
import shutil
import os

# 创建一个空文件，此文件作为apk包中的空文件
src_empty_file = './info/empty.txt'
f = open(src_empty_file, 'w')
f.close()

# 在渠道号配置文件中，获取指定的渠道号
channelFile = open('./info/channel.txt', 'r', -1, 'UTF-8')
channels = channelFile.readlines()
channelFile.close()
print('-' * 20, 'all channels', '-' * 20)
print(channels)
print('-' * 50)

# 获取当前目录下所有的apk文件
src_apks = []
for file in os.listdir('./package'):
    print('file:' + file)
    if os.path.isfile('./package/' + file):
        extension = os.path.splitext(file)[1][1:]
        print('extension:' + extension)
        if extension == 'apk':
            src_apks.append('./package/' + file)
            print('append file:' + file)

# 遍历所以的apk文件，向其压缩文件中添加渠道号文件
for src_apk in src_apks:
    src_apk_file_name = os.path.basename(src_apk)
    print('current apk name:', src_apk_file_name)
    temp_list = os.path.splitext(src_apk_file_name)
    src_apk_name = temp_list[0]
    src_apk_extension = temp_list[1]

    output_dir = 'outputDir/' + src_apk_name + '/'
    if not os.path.exists(output_dir):
        os.mkdir(output_dir)
    for file in os.listdir(output_dir):
        os.remove(output_dir + file)

    # 遍历从文件中获得的所以渠道号，将其写入APK包中
    for line in channels:
        target_channel = line.strip()
        target_apk = output_dir + src_apk_name + \
            "_" + target_channel + src_apk_extension
        shutil.copy(src_apk, target_apk)
        zipped = zipfile.ZipFile(target_apk, 'a', zipfile.ZIP_DEFLATED)
        empty_channel_file = "META-INF/Channel@{channel}".format(
            channel=target_channel)
        zipped.write(src_empty_file, empty_channel_file)
        zipped.close()

print('-' * 50)
print('repackaging is over ,total package: ', len(channels))
