from azure.servicebus import ServiceBusService, Message, Topic, Rule, DEFAULT_RULE_NAME
 
bus_service = ServiceBusService(
service_namespace='dfmessage',
shared_access_key_name='meu_token',
shared_access_key_value='+JWuf875RyazD1Cj8/ezM49LiPk08c+B0lm/I4nqx98=')

topic_name= 'df.magalu.challenge'
consumer_name = 'teste_python_bolado'

 
for i in range(1):
    msg = Message('nova Msg {0}'.format(i).encode('utf-8'),
    custom_properties={'messageposition': i})
    bus_service.send_topic_message(topic_name, msg)
    print("enviada! ",i)
    

# bus_service.create_subscription(topic_name, consumer_name)
# while True:
#     msg = bus_service.receive_subscription_message(topic_name, consumer_name, peek_lock=False)
#     print(msg.body)