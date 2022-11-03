//base url: https://www.tektutorialshub.com/angular/angular-preloading-strategy/

import { Injectable } from '@angular/core';
import { Observable, of, timer } from 'rxjs';
import { mergeMap } from 'rxjs/operators'

import { PreloadingStrategy, Route } from '@angular/router';

@Injectable()
export class CustomPreloadingStrategy implements PreloadingStrategy {

  preload(route: Route, load: () => Observable<any>): Observable<any> {

    if (route.data && route.data['preload']) {
      var delay: number = route.data['delay']
      if (route.data['delay']) {
        return timer(delay).pipe(
          mergeMap(_ => {
            return load();
          }));
      }
      return load();
    } else {
      return of(null);
    }
  }

}
