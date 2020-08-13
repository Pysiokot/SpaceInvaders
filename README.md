# SpaceInvaders

Dzień dobry.

W tym projekcie pierwszy raz skorzystałem z Zenjecta do Dependency Injection co wydaje mi się dość fajnym rozwiązaniem, które otworzyło mi oczy, aby niektóre tematy ugryźć inaczej
niż początkowo planowałem.

Niestety są w tym projekcie trzy rzeczy, z których nie jestem zadowolony. Są to:
* Logika stojąca za strzelaniem przeciwników (wydawaniem im pozwoleń na strzelanie) -> jest zrobiona bardzo bruteforce'owo - powinno być przez inną klasę, albo przez przeciwników samych w sobie;
* GridEnemySpawner -> Z tej klasy całkowicie powinna być usunięta funkcjonalność, która wydaje przeciwnikom pozwolenie na strzelanie - powinno zostać samo spawnowanie jak nazwa klasy wskazuje;
* EnemyGroupMover -> w tym projekcie ze względu na specyfikację z początku stwierdziłem, że będę poruszać parentem wszystkich przeciwników dzięki czemu cała grupa będzie się poruszała tak jak chcę. Niestety to rozwiązanie jest non-flexible. Każdy z przeciwników powinien mieć zaszytą logikę poruszania, dzięki czemu po zmianie interfejsu dostarczającego takiej funkcjonalności można by łatwo zmienić tą grę w jakiegoś asteroid shootera, czy też Alien shootera.

TODO:
niektóre z nich są w kodzie (raczej te wymienione powyżej), ale brakuje też
1. Animacji wybuchania przeciwników, która mogła by być zrobiona analogicznie do animacji, które są na graczu;
2. Efektów rozwalania osłon - każda ma po 5 żyć i nie ma żadnego feedbacku - mógłbym jakiś shader z maską napisać.


Jak widać nie zrobiłem wszystkiego co chciałem. Wynika to przede wszystkim z tego, że jeszcze jestem w trakcie pisania pracy magisterskiej, na którą mam coraz mniej czasu oraz staram się skorzystać z urlopu.
