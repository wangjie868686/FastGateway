docker build -f ./src/FastGateway/Dockerfile -t registry.cn-shenzhen.aliyuncs.com/tokengo/gateway-api:v1.0.0 .

docker push  registry.cn-shenzhen.aliyuncs.com/tokengo/gateway-api:v1.0.0

docker build -f ./src/FastGateway/Dockerfile-h3 -t registry.cn-shenzhen.aliyuncs.com/tokengo/gateway-api:v1.0.0-h3 .

docker push  registry.cn-shenzhen.aliyuncs.com/tokengo/gateway-api:v1.0.0-h3