import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShortUrlInfo } from './short-url-info';

describe('ShortUrlInfo', () => {
  let component: ShortUrlInfo;
  let fixture: ComponentFixture<ShortUrlInfo>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShortUrlInfo],
    }).compileComponents();

    fixture = TestBed.createComponent(ShortUrlInfo);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
