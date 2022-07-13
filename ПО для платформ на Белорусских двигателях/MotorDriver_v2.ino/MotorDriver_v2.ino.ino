//07.2021
//
//$,F,255,B,255,B,255 - вид сообщения. 
//Направление вращения ("F", "B")("Forvard", "Back"). "Вперёд", "назад" (условно вверх/вниз)
//Скорость вращения (0-255).

//#,R - вид сообщения
//R - красный, G - зелёный, B - синий, D - выключить всё

//@,AL
//AL - авария, NO - отключение аварии

//Алгоритм запускает/останавливает двигатели или изменяет их скорость 
//и направление вращения. Выставляет необходимый уровень ШИМ на пинах 
//управления скоростью. Скорость равная нулю - двигатели "стоп".

//В коде происходит обработка состояния концевых выключателей и отключение двигателей при их срабатывании

#include <Wire.h>

#define error 1
#define NO_error 0

//концевики
#define IN1 A0
#define IN2 A1
#define IN3 A2
#define IN4 11
#define IN5 12
#define IN6 13
#define inALARM A3//аварийная кнопка

//свтодиодная лента
//#define BOARD_EXPANSION_ADDR 0x38//базовый адрес для микросхем расширения входов типа PCA8574AD
#define BOARD_EXPANSION_ADDR 0x20//базовый адрес для микросхем расширения входов типа PCF8574T

//вспомогательные флаги концевиков для однократной фиксации изменения состояния
bool flag_IN1 = false;
bool flag_IN2 = false;
bool flag_IN3 = false;
bool flag_IN4 = false;
bool flag_IN5 = false;
bool flag_IN6 = false;
bool flag_inALARM = false;//флаг аварийной кнопки

//основные флаги состояния концевиков
bool flagLimitSensor_1_Top = false;//верхний концевик первого двигателя
bool flagLimitSensor_1_Bottom = false;//нижний концевик первого двигателя
bool flagLimitSensor_2_Top = false;
bool flagLimitSensor_2_Bottom = false;
bool flagLimitSensor_3_Top = false;
bool flagLimitSensor_3_Bottom = false;
bool flagALARMSensor = false;

uint32_t btnTimer = 0;//вспомогательная для обработки дребезга

const uint8_t EN_1 = 3; //шим первого двигателя
const uint8_t EN_2 = 5; //шим второго двигателя
const uint8_t EN_3 = 6; //шим третьего двигателя

const uint8_t L_PWM_1 = 2; //направление вращения первого двигателя      
const uint8_t R_PWM_1 = 4;

const uint8_t L_PWM_2 = 7; //направление вращения второго двигателя      
const uint8_t R_PWM_2 = 8;

const uint8_t L_PWM_3 = 9; //направление вращения третьего двигателя      
const uint8_t R_PWM_3 = 10;
  
unsigned int speed_1Int = 0;//переменные скорости
unsigned int speed_2Int = 0;
unsigned int speed_3Int = 0;

String direction_1Str, direction_2Str, direction_3Str;//переменные направления

char direction_1[10];//битовые массивы для разбора принятой строки
char direction_2[10];
char direction_3[10];

char speed_1[5];
char speed_2[5];
char speed_3[5];

char prefix[1];

unsigned int STATUS;//для фиксации ошибок входящих данных

byte outPinsStatus = 0b11111111;//0xFF//состояние выходов платы расширения

//для мигания красным при аварии
bool flagBlink = true;
int ch = 0;
bool flagB_ = false;

void setup() {
  Serial.begin(115200);

  Wire.begin();//инициализация линии I2C
  Wire.beginTransmission(BOARD_EXPANSION_ADDR);
  Wire.write(outPinsStatus);//выставляем начальное значение
  Wire.endTransmission();
  
  pinMode(EN_1, OUTPUT);
  pinMode(EN_2, OUTPUT);
  pinMode(EN_3, OUTPUT);
  pinMode(L_PWM_1, OUTPUT);
  pinMode(R_PWM_1, OUTPUT);
  pinMode(L_PWM_2, OUTPUT);
  pinMode(R_PWM_2, OUTPUT);
  pinMode(L_PWM_3, OUTPUT);
  pinMode(R_PWM_3, OUTPUT);

  pinMode(IN1, INPUT_PULLUP);
  pinMode(IN2, INPUT_PULLUP);
  pinMode(IN3, INPUT_PULLUP);
  pinMode(IN4, INPUT_PULLUP);
  pinMode(IN5, INPUT_PULLUP);
  pinMode(IN6, INPUT_PULLUP);
  pinMode(inALARM, INPUT_PULLUP);

  //greenON();
  blueON();
  delay(3);
}

void loop() 
{
  int i=0;
  char port_buffer[20];

  ch++;//счётчик для мигания
  if(ch > 10000) ch = 0;//сброс счётчика

  //ОБРАБОТКА КОНЦЕВИКОВ
  //IN1
  if(!digitalRead(IN1) && !flag_IN1 && millis() - btnTimer > 100){
    flag_IN1 = true;
    btnTimer = millis();
    Serial.println("1ON");
    flagLimitSensor_1_Top = true;//верхний концевик первого двигателя сработал    
  }
  else if(digitalRead(IN1) && flag_IN1 && millis() - btnTimer > 100){
    flag_IN1 = false;
    btnTimer = millis();
    Serial.println("1OFF");
    flagLimitSensor_1_Top = false;//верхний концевик первого двигателя отпущен
  }
  //IN2
  if(!digitalRead(IN2) && !flag_IN2 && millis() - btnTimer > 100){
    flag_IN2 = true;
    btnTimer = millis();
    Serial.println("2ON");    
    flagLimitSensor_1_Bottom = true;//нижний концевик первого двигателя сработал 
  }
  else if(digitalRead(IN2) && flag_IN2 && millis() - btnTimer > 100){
    flag_IN2 = false;
    btnTimer = millis();
    Serial.println("2OFF");
    flagLimitSensor_1_Bottom = false;//нижний концевик первого двигателя отпущен
  }
  //IN3
  if(!digitalRead(IN3) && !flag_IN3 && millis() - btnTimer > 100){
    flag_IN3 = true;
    btnTimer = millis();
    Serial.println("3ON"); 
    flagLimitSensor_2_Top = true;   
  }
  else if(digitalRead(IN3) && flag_IN3 && millis() - btnTimer > 100){
    flag_IN3 = false;
    btnTimer = millis();
    Serial.println("3OFF");
    flagLimitSensor_2_Top = false;
  }
  //IN4
  if(!digitalRead(IN4) && !flag_IN4 && millis() - btnTimer > 100){
    flag_IN4 = true;
    btnTimer = millis();
    Serial.println("4ON");    
    flagLimitSensor_2_Bottom = true;
  }
  else if(digitalRead(IN4) && flag_IN4 && millis() - btnTimer > 100){
    flag_IN4 = false;
    btnTimer = millis();
    Serial.println("4OFF");
    flagLimitSensor_2_Bottom = false;
  }
  //IN5
  if(!digitalRead(IN5) && !flag_IN5 && millis() - btnTimer > 100){
    flag_IN5 = true;
    btnTimer = millis();
    Serial.println("5ON"); 
    flagLimitSensor_3_Top = true;    
  }
  else if(digitalRead(IN5) && flag_IN5 && millis() - btnTimer > 100){
    flag_IN5 = false;
    btnTimer = millis();
    Serial.println("5OFF");
    flagLimitSensor_3_Top = false;
  }
  //IN6
  if(!digitalRead(IN6) && !flag_IN6 && millis() - btnTimer > 100){
    flag_IN6 = true;
    btnTimer = millis();
    Serial.println("6ON");
    flagLimitSensor_3_Bottom = true;    
  }
  else if(digitalRead(IN6) && flag_IN6 && millis() - btnTimer > 100){
    flag_IN6 = false;
    btnTimer = millis();
    Serial.println("6OFF");
    flagLimitSensor_3_Bottom = false;
  }
  //inALARM
  if(digitalRead(inALARM) && !flag_inALARM && millis() - btnTimer > 100){
    flag_inALARM = true;
    alarmON();
    Serial.print("ALARMON");    
  }
  else if(!digitalRead(inALARM) && flag_inALARM && millis() - btnTimer > 100){
    flag_inALARM = false;
    alarmOFF();
    Serial.print("ALARMOFF");
  }

  //ОБРАБОТКА ДАННЫХ ИЗ COM ПОРТА
  if(Serial.available())//если есть данные - считаем их
  {
     delay(3);//ожидаем приёма всех данных
    
     while(Serial.available() && i<20) 
     { 
        port_buffer[i++] = Serial.read();//сохраним прочитанное в буфер        
        if(!Serial.available() && i<20)
        {
           port_buffer[i++] = 0;
        }
      }
    
      if(port_buffer[0] == '$')//УПРАВЛЕНИЕ ДВИГАТЕЛЯМИ      
      {  
          sscanf(port_buffer, "%[^','], %[^','], %[^','], %[^','], %[^','], %[^','], %s", &prefix, &direction_1, &speed_1, &direction_2, &speed_2, &direction_3, &speed_3);//разобьем его на части, отделённые запятой
          STATUS = NO_error;
          
          direction_1Str = String(direction_1);
          direction_2Str = String(direction_2);
          direction_3Str = String(direction_3);
    
          speed_1Int = String(speed_1).toInt();
          speed_2Int = String(speed_2).toInt();
          speed_3Int = String(speed_3).toInt();
         
          if(direction_1Str == "B"){
            digitalWrite(L_PWM_1, LOW);
            digitalWrite(R_PWM_1, HIGH);
          }
          else if(direction_1Str == "F"){
            digitalWrite(L_PWM_1, HIGH);
            digitalWrite(R_PWM_1, LOW);
          }
          else{
            STATUS = error; 
            Serial.println("ERROR_1");
          }
          //---------------------------
          if(direction_2Str == "B"){
            digitalWrite(L_PWM_2, LOW);
            digitalWrite(R_PWM_2, HIGH);
          }
          else if(direction_2Str == "F"){
            digitalWrite(L_PWM_2, HIGH);
            digitalWrite(R_PWM_2, LOW);
          }
          else{
            STATUS = error; 
            Serial.println("ERROR_2");
          }
          //---------------------------
          if(direction_3Str == "B"){
            digitalWrite(L_PWM_3, LOW);
            digitalWrite(R_PWM_3, HIGH);
          }
          else if(direction_3Str == "F"){
            digitalWrite(L_PWM_3, HIGH);
            digitalWrite(R_PWM_3, LOW);
          }
          else{
            STATUS = error; 
            Serial.println("ERROR_3");
          }
  
          if(speed_1Int < 0 || speed_1Int > 255){STATUS = error;}
          if(speed_2Int < 0 || speed_2Int > 255){STATUS = error;}
          if(speed_3Int < 0 || speed_3Int > 255){STATUS = error;}
        
          /*Serial.println(direction_1Str);
          Serial.println(speed_1Int);
          Serial.println(direction_2Str);
          Serial.println(speed_2Int);
          Serial.println(direction_3Str);
          Serial.println(speed_3Int);*/
       }
       else if(port_buffer[0] == '#')//УПРАВЛЕНИЕ СВЕТОДИОДНОЙ ЛЕНТОЙ
       {
          switch(port_buffer[2])
          {
             case 'R':
               redON();
               break;
             case 'G':
               greenON();
               break;
             case 'B':
               blueON();
               break;
             case 'D':
               allPinsOFF();
               break;
          }
        /*Serial.println(buffer[0]);
        Serial.println(buffer[2]);*/
       }
       else if(port_buffer[0] == '%')//ОБРАБОТКА СООБЩЕНИЙ ОБ АВАРИИ И ЕЁ ОТМЕНЕ
       {
          if(port_buffer[2] == 'A' && port_buffer[4] == 'L')
            alarmON();
            
          if(port_buffer[2] == 'N' && port_buffer[4] == 'O')
            alarmOFF();            
       }          
  }

  //помигаем красным при аварии
        if(flagALARMSensor)
        {
           if(flagBlink && ch == 5000)
           {
              //allPinsOFF();
              //redON();
              redON_alarm();
              flagBlink = false;
           }
           else if(!flagBlink && ch == 10000)
           {
              //allPinsOFF();
              redOFF_alarm();
              flagBlink = true;
           }
           flagB_ = false;
        }
        /*else if(!flagALARMSensor && !flagB_)//механизм однократного включения зелёного после снятия сигнала аварии
        {
            flagB_ = true;
            allPinsOFF();
            delay(5);
            greenON();
        }*/

//УПРАВЛЕНИЕ ДВИГАТЕЛЯМИ ПО КОНЦЕВИКАМ
  if(STATUS == NO_error && !flagALARMSensor)
  {
  //первый вверх
     if(direction_1Str == "B"){//если направление вверх
        if(!flagLimitSensor_1_Top)//если верхний концевик не нажат, 
           analogWrite(EN_1, speed_1Int);//то устанавливаем заданную скорость        
        else if(flagLimitSensor_1_Top)//если верхний концевик нажат,        
           M1_Stop();              
     }
     //первый вниз
     if(direction_1Str == "F"){//если направление вниз     
        if(!flagLimitSensor_1_Bottom) 
           analogWrite(EN_1, speed_1Int);                    
        else if(flagLimitSensor_1_Bottom) 
           M1_Stop();                                     
     }
     //-----------------------------------------------------------------------------------
     //второй вверх
     if(direction_2Str == "B"){
        if(!flagLimitSensor_2_Top) 
           analogWrite(EN_2, speed_2Int);
        else if(flagLimitSensor_2_Top) 
           M2_Stop();
     }
     //второй вниз
     if(direction_2Str == "F"){
        if(!flagLimitSensor_2_Bottom) 
           analogWrite(EN_2, speed_2Int);
        else if(flagLimitSensor_2_Bottom) 
           M2_Stop();;
     }
     //-----------------------------------------------------------------------------------
     //третий вверх
     if(direction_3Str == "B"){
        if(!flagLimitSensor_3_Top) 
           analogWrite(EN_3, speed_3Int);
        else if(flagLimitSensor_3_Top) 
           M3_Stop();
     }
     //третий вниз  
     if(direction_3Str == "F"){
         if(!flagLimitSensor_3_Bottom) 
            analogWrite(EN_3, speed_3Int);
         else if(flagLimitSensor_3_Bottom)
           M3_Stop();
     }
  }
  else//если ошибка при приёме данных или авария
  {
     M1_Stop();
     M2_Stop();
     M3_Stop();
     speed_1Int = 0;
     speed_2Int = 0;
     speed_3Int = 0;
     //Serial.println("ERROR_4");
  }
}

void board_expansion_write(byte data){  
  Wire.beginTransmission(BOARD_EXPANSION_ADDR);
  Wire.write(data);
  Wire.endTransmission();
}

void alarmON(){
    M1_Stop();
    M2_Stop();
    M3_Stop();
     
    btnTimer = millis();
    flagALARMSensor = true;//авария
    allPinsOFF();//гасим LED
    //outPinsStatus = 0xF6;//0b11110110 pin3 - контактор, pin0 - красный
    //board_expansion_write(outPinsStatus);
}

void alarmOFF(){
    btnTimer = millis();
    flagALARMSensor = false;
    allPinsOFF();
    delay(50);
    greenON();
}

void redON(){
  outPinsStatus &= ~(1<<0);
  board_expansion_write(outPinsStatus);
}

void greenON(){
  outPinsStatus &= ~(1<<1);
  board_expansion_write(outPinsStatus);
}

void blueON(){
  outPinsStatus &= ~(1<<2);
  board_expansion_write(outPinsStatus);
}

void allPinsOFF(){
  outPinsStatus = 0xFF;
  board_expansion_write(outPinsStatus);
}

void redON_alarm(){
  //outPinsStatus = 0b11110110;//с контактором
  outPinsStatus = 0b11111110;
  board_expansion_write(outPinsStatus);
}

void redOFF_alarm(){
  //outPinsStatus = 0b11110111;//с контактором
  outPinsStatus = 0b11111111;
  board_expansion_write(outPinsStatus);
}

void M1_Stop(){
  analogWrite(EN_1, 0);
  digitalWrite(L_PWM_1, LOW);
  digitalWrite(R_PWM_1, LOW);
}

void M2_Stop(){
  analogWrite(EN_2, 0);
  digitalWrite(L_PWM_2, LOW);
  digitalWrite(R_PWM_2, LOW);
}

void M3_Stop(){
  analogWrite(EN_3, 0);
  digitalWrite(L_PWM_3, LOW);
  digitalWrite(R_PWM_3, LOW);
}
