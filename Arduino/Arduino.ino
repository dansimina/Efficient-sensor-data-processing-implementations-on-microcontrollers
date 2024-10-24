char c;
String appendSerialData;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
}

void loop() {
  // put your main code here, to run repeatedly:
  
  while(Serial.available() > 0) {
    c = Serial.read();
    appendSerialData += c;
  }
  if(c == '#') {
    Serial.print("Arduino >> ");
    Serial.println(appendSerialData);
    appendSerialData = "";
  }
}
