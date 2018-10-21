# seatSelecter


### 长连接交互方式说明

> 1. 这是采用 3 种消息机制，CGI, PUSH, SYNC
> 2. CGI: 客户端推送消息到服务器，服务器返回消息给客户端
> 3. PUSH: 客户端推送消息到服务器, 服务器不给出任何回应
> 4. SYNC: 服务器推送消息到客户端

### 网络消息格式说明

> 1. 在包中说明消息类型：{"type": "CGI/PUSH/SYNC"}
> 2. 在包中加入指令：{"command": "xxxxx"}

### 接口
> 1. 选择座位
```
     client: {
                "type": "CGI", 
                "command": "choose",
                "seatID": 0/1/2/3 .. 31
             }
     server: {
              "type": "CGI",
              "command": "choose",
              "result": "success" | "error reason"
             }
```
> 2. 设置姓名
```
     client: {
              "type": "CGI",
              "command": "setName",
              "name": UTF-8
             }
     server: {
              "type": "CGI",
              "command": "setName",
              "result": "success" | "error reason"
             }
```
> 3. 更新座位表
```
     server: {
                "type": "SYNC",
                "command": "seatStatus"
                "0": UTF-8,   // 座位id对应姓名，如果为 "NULL", 表示该座位上没人
                "1": UTF-8,
                "2": UTF-8,
                ...
                "31": UTF-8
             }
```