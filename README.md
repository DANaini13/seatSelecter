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
> 1. 
```
     client: {
                "type": "CGI", 
                "command": "choose",
                "seatID": 0/1/2/3 .. 31
             }
     server: {
              "type": "CGI",
              "command": "choose",
              "result": 0/1
             }

> 2. 
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

> 3. 
```
     server: {
                "type": "SYNC",
                "seatStatus": [
                    "0": 0/1,
                    "1": 0/1,
                    "2": 0/1,
                    ...
                    "31": 0/1
                ]
             }