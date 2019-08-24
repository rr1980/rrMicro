import { Component, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

const headers = new HttpHeaders({
  'Content-Type': 'application/json',
  //'Accept': '*/*',
});


@Component({
  selector: 'app-counter-component',
  templateUrl: './counter.component.html'
})
export class CounterComponent {

  public datas: string[];
  private token: string;
  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    ;
  }

  login() {

    this.http.post<any>('http://localhost:5000/Login', { username: "Test1", password: "Pass1" }, { headers, responseType: 'text' as 'json' }).subscribe(token => {
      console.debug("token", token);
      this.token = token;
    }, error => {
      //console.error(error);
      throw error;
    });
  }

  get() {
    var _headers = headers.set('token', this.token);

    this.http.get<string[]>('http://localhost:5000/Test/Get', { headers: _headers }).subscribe(datas => {
      this.datas = datas;
      console.debug("datas", datas);
    }, error => {
      //console.error(error);
      throw error;
    });
  }
}
